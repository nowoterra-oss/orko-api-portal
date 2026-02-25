using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Constants;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Infrastructure.BackgroundJobs;

public class WorkOrderEmailNotificationJob
{
    private readonly PortalDbContext _db;
    private readonly IEmailService _email;
    private readonly ILogger<WorkOrderEmailNotificationJob> _logger;

    public WorkOrderEmailNotificationJob(
        PortalDbContext db, IEmailService email, ILogger<WorkOrderEmailNotificationJob> logger)
    {
        _db = db;
        _email = email;
        _logger = logger;
    }

    [Hangfire.AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    public async Task ExecuteAsync(Guid workOrderId)
    {
        var settings = await _db.AppSettings
            .Where(s => s.Category == SettingKeys.CategoryWorkOrderEmail)
            .ToListAsync();

        var dict = settings.ToDictionary(s => s.Key, s => s.Value ?? "");

        var enabled = dict.GetValueOrDefault(SettingKeys.WorkOrderEmailEnabled, "false");
        if (!enabled.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Is emri e-posta bildirimi devre disi, atlanıyor.");
            return;
        }

        var recipientsCsv = dict.GetValueOrDefault(SettingKeys.WorkOrderEmailRecipients, "");
        var recipients = recipientsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (recipients.Length == 0)
        {
            _logger.LogWarning("Is emri e-posta bildirimi icin alici tanimlanmamis.");
            return;
        }

        var workOrder = await _db.WorkOrders.FirstOrDefaultAsync(w => w.Id == workOrderId);
        if (workOrder == null)
        {
            _logger.LogWarning("Is emri bulunamadi: {WorkOrderId}", workOrderId);
            return;
        }

        var subject = dict.GetValueOrDefault(SettingKeys.WorkOrderEmailSubject, "Yeni Is Emri: {FileNumber}");
        var template = dict.GetValueOrDefault(SettingKeys.WorkOrderEmailTemplate, DefaultTemplate);

        var customerName = ExtractCustomerName(workOrder.SelsilPayload) ?? "-";

        var body = template
            .Replace("{FileNumber}", workOrder.FileNumber)
            .Replace("{CustomerName}", customerName)
            .Replace("{Type}", workOrder.Type.ToString())
            .Replace("{CreatedAt}", workOrder.CreatedAt.ToString("dd.MM.yyyy HH:mm"));

        subject = subject
            .Replace("{FileNumber}", workOrder.FileNumber)
            .Replace("{CustomerName}", customerName);

        await _email.SendEmailAsync(recipients, subject, body);

        _logger.LogInformation(
            "Is emri e-posta bildirimi gonderildi: {FileNumber} -> {Recipients}",
            workOrder.FileNumber, recipientsCsv);
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
        <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <div style="background-color: #2563eb; color: white; padding: 20px; border-radius: 8px 8px 0 0;">
                <h2 style="margin: 0;">Yeni İş Emri Oluşturuldu</h2>
            </div>
            <div style="padding: 20px; border: 1px solid #e5e7eb; border-top: none; border-radius: 0 0 8px 8px;">
                <table style="width: 100%; border-collapse: collapse;">
                    <tr><td style="padding: 8px 0; color: #6b7280;">Dosya No:</td><td style="padding: 8px 0; font-weight: bold;">{FileNumber}</td></tr>
                    <tr><td style="padding: 8px 0; color: #6b7280;">Müşteri:</td><td style="padding: 8px 0;">{CustomerName}</td></tr>
                    <tr><td style="padding: 8px 0; color: #6b7280;">Tip:</td><td style="padding: 8px 0;">{Type}</td></tr>
                    <tr><td style="padding: 8px 0; color: #6b7280;">Tarih:</td><td style="padding: 8px 0;">{CreatedAt}</td></tr>
                </table>
                <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 16px 0;" />
                <p style="color: #6b7280; font-size: 12px; margin: 0;">Bu e-posta Orko Portal tarafından otomatik olarak gönderilmiştir.</p>
            </div>
        </div>
        """;
}
