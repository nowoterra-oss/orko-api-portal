using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Contracts.Settings;
using Orko.Portal.Domain.Constants;
using Orko.Portal.Domain.Entities;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.BackgroundJobs;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Settings;

public class SettingsHandler
{
    private readonly PortalDbContext _db;
    private readonly IEmailService _email;
    private readonly IRecurringJobManager _recurringJobs;
    private readonly ILogger<SettingsHandler> _logger;

    public SettingsHandler(
        PortalDbContext db,
        IEmailService email,
        IRecurringJobManager recurringJobs,
        ILogger<SettingsHandler> logger)
    {
        _db = db;
        _email = email;
        _recurringJobs = recurringJobs;
        _logger = logger;
    }

    public async Task<List<AppSettingDto>> GetAllAsync()
    {
        var settings = await _db.AppSettings.OrderBy(s => s.Category).ThenBy(s => s.Key).ToListAsync();
        return settings.Select(s => new AppSettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = MaskSensitive(s.Key, s.Value),
            Description = s.Description,
            Category = s.Category,
            UpdatedAt = s.UpdatedAt,
            UpdatedBy = s.UpdatedBy
        }).ToList();
    }

    public async Task<List<AppSettingDto>> GetByCategoryAsync(string category)
    {
        var settings = await _db.AppSettings
            .Where(s => s.Category == category)
            .OrderBy(s => s.Key)
            .ToListAsync();

        return settings.Select(s => new AppSettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = MaskSensitive(s.Key, s.Value),
            Description = s.Description,
            Category = s.Category,
            UpdatedAt = s.UpdatedAt,
            UpdatedBy = s.UpdatedBy
        }).ToList();
    }

    public async Task UpdateAsync(UpdateSettingsDto dto, string updatedBy)
    {
        var existingSettings = await _db.AppSettings.ToListAsync();
        var existingDict = existingSettings.ToDictionary(s => s.Key);

        foreach (var item in dto.Settings)
        {
            // Masked password — skip update
            if (item.Key == SettingKeys.SmtpPassword && item.Value == "********")
                continue;

            if (existingDict.TryGetValue(item.Key, out var existing))
            {
                existing.Value = item.Value;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = updatedBy;
            }
            else
            {
                _db.AppSettings.Add(new AppSetting
                {
                    Key = item.Key,
                    Value = item.Value,
                    Description = item.Description,
                    Category = item.Category,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = updatedBy
                });
            }
        }

        await _db.SaveChangesAsync();

        // Reminder recurring job güncelle
        SyncReminderJob(dto.Settings);

        _logger.LogInformation("Ayarlar guncellendi: {Count} ayar, Guncelleyen: {UpdatedBy}",
            dto.Settings.Count, updatedBy);
    }

    public async Task<SmtpTestResultDto> TestSmtpAsync()
    {
        try
        {
            var success = await _email.TestConnectionAsync();
            return new SmtpTestResultDto
            {
                Success = success,
                Message = success ? "SMTP bağlantısı başarılı. Test e-postası gönderildi." : "SMTP bağlantısı başarısız."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP test hatasi");
            return new SmtpTestResultDto
            {
                Success = false,
                Message = $"SMTP bağlantı hatası: {ex.Message}"
            };
        }
    }

    private void SyncReminderJob(List<SettingItemDto> settings)
    {
        var reminderEnabled = settings.FirstOrDefault(s => s.Key == SettingKeys.ReminderEnabled)?.Value;
        var cronExpression = settings.FirstOrDefault(s => s.Key == SettingKeys.ReminderCronExpression)?.Value;

        if (reminderEnabled == null && cronExpression == null)
            return; // Reminder ayarları değişmemiş

        if (reminderEnabled?.Equals("true", StringComparison.OrdinalIgnoreCase) == true
            && !string.IsNullOrEmpty(cronExpression))
        {
            _recurringJobs.AddOrUpdate<DraftReminderJob>(
                "draft-reminder",
                job => job.ExecuteAsync(),
                cronExpression);
            _logger.LogInformation("Hatirlatma recurring job guncellendi: {Cron}", cronExpression);
        }
        else if (reminderEnabled?.Equals("false", StringComparison.OrdinalIgnoreCase) == true)
        {
            _recurringJobs.RemoveIfExists("draft-reminder");
            _logger.LogInformation("Hatirlatma recurring job kaldirildi");
        }
    }

    private static string? MaskSensitive(string key, string? value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        if (key == SettingKeys.SmtpPassword)
            return "********";
        return value;
    }
}
