using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Constants;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Infrastructure.BackgroundJobs;

public class DraftReminderJob
{
    private readonly PortalDbContext _db;
    private readonly IEmailService _email;
    private readonly ILogger<DraftReminderJob> _logger;

    public DraftReminderJob(
        PortalDbContext db, IEmailService email, ILogger<DraftReminderJob> logger)
    {
        _db = db;
        _email = email;
        _logger = logger;
    }

    [Hangfire.AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    public async Task ExecuteAsync()
    {
        var settings = await _db.AppSettings
            .Where(s => s.Category == SettingKeys.CategoryReminder)
            .ToListAsync();

        var dict = settings.ToDictionary(s => s.Key, s => s.Value ?? "");

        var enabled = dict.GetValueOrDefault(SettingKeys.ReminderEnabled, "false");
        if (!enabled.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Hatirlatma servisi devre disi, atlaniyor.");
            return;
        }

        var recipientsCsv = dict.GetValueOrDefault(SettingKeys.ReminderRecipients, "");
        var recipients = recipientsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (recipients.Length == 0)
        {
            _logger.LogWarning("Hatirlatma e-postasi icin alici tanimlanmamis.");
            return;
        }

        var thresholdHours = int.TryParse(dict.GetValueOrDefault(SettingKeys.ReminderDraftThresholdHours, "24"), out var h) ? h : 24;
        var threshold = DateTime.UtcNow.AddHours(-thresholdHours);

        // Taslak durumunda ve Evrim'e gönderilmemiş iş emirlerini bul
        var draftOrders = await _db.WorkOrders
            .Include(w => w.Declaration)
            .Where(w => w.Status == WorkOrderStatus.Taslak
                && w.CreatedAt < threshold
                && (w.Declaration == null || !w.Declaration.SentToEvrim))
            .OrderBy(w => w.CreatedAt)
            .ToListAsync();

        if (draftOrders.Count == 0)
        {
            _logger.LogInformation("Hatirlatma: bekleyen taslak is emri yok.");
            return;
        }

        var subject = dict.GetValueOrDefault(SettingKeys.ReminderSubject, "Taslak İş Emirleri Hatırlatması ({Count} adet)");
        var template = dict.GetValueOrDefault(SettingKeys.ReminderTemplate, DefaultTemplate);

        // HTML tablo satırları oluştur
        var tableRows = new StringBuilder();
        foreach (var order in draftOrders)
        {
            var customerName = ExtractCustomerName(order.SelsilPayload) ?? "-";
            var waitingHours = (int)(DateTime.UtcNow - order.CreatedAt).TotalHours;
            tableRows.AppendLine($"""
                <tr>
                    <td style="padding: 8px; border: 1px solid #e5e7eb;">{order.FileNumber}</td>
                    <td style="padding: 8px; border: 1px solid #e5e7eb;">{customerName}</td>
                    <td style="padding: 8px; border: 1px solid #e5e7eb;">{order.Type}</td>
                    <td style="padding: 8px; border: 1px solid #e5e7eb;">{order.CreatedAt:dd.MM.yyyy HH:mm}</td>
                    <td style="padding: 8px; border: 1px solid #e5e7eb;">{waitingHours} saat</td>
                </tr>
                """);
        }

        subject = subject.Replace("{Count}", draftOrders.Count.ToString());

        var body = template
            .Replace("{Count}", draftOrders.Count.ToString())
            .Replace("{TableRows}", tableRows.ToString());

        await _email.SendEmailAsync(recipients, subject, body);

        _logger.LogInformation(
            "Hatirlatma e-postasi gonderildi: {Count} taslak is emri -> {Recipients}",
            draftOrders.Count, recipientsCsv);
    }

    private static string? ExtractCustomerName(string? payload)
    {
        if (string.IsNullOrEmpty(payload)) return null;
        try
        {
            var doc = JsonDocument.Parse(payload);
            if (doc.RootElement.TryGetProperty("CustomerName", out var prop))
                return prop.GetString();
        }
        catch { }
        return null;
    }

    private const string DefaultTemplate = """
        <div style="font-family: Arial, sans-serif; max-width: 800px; margin: 0 auto;">
            <div style="background-color: #f59e0b; color: white; padding: 20px; border-radius: 8px 8px 0 0;">
                <h2 style="margin: 0;">Taslak İş Emirleri Hatırlatması</h2>
                <p style="margin: 8px 0 0 0; opacity: 0.9;">{Count} adet iş emri beyanname gönderimi bekliyor</p>
            </div>
            <div style="padding: 20px; border: 1px solid #e5e7eb; border-top: none; border-radius: 0 0 8px 8px;">
                <table style="width: 100%; border-collapse: collapse;">
                    <thead>
                        <tr style="background-color: #f9fafb;">
                            <th style="padding: 8px; border: 1px solid #e5e7eb; text-align: left;">Dosya No</th>
                            <th style="padding: 8px; border: 1px solid #e5e7eb; text-align: left;">Müşteri</th>
                            <th style="padding: 8px; border: 1px solid #e5e7eb; text-align: left;">Tip</th>
                            <th style="padding: 8px; border: 1px solid #e5e7eb; text-align: left;">Oluşturma</th>
                            <th style="padding: 8px; border: 1px solid #e5e7eb; text-align: left;">Bekleme</th>
                        </tr>
                    </thead>
                    <tbody>
                        {TableRows}
                    </tbody>
                </table>
                <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 16px 0;" />
                <p style="color: #6b7280; font-size: 12px; margin: 0;">Bu e-posta Orko Portal hatırlatma servisi tarafından otomatik olarak gönderilmiştir.</p>
            </div>
        </div>
        """;
}
