using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Constants;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly PortalDbContext _db;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(PortalDbContext db, ILogger<SmtpEmailService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SendEmailAsync(IEnumerable<string> to, string subject, string htmlBody)
    {
        var settings = await GetSmtpSettingsAsync();

        using var client = CreateSmtpClient(settings);
        using var message = new MailMessage();

        message.From = new MailAddress(settings.FromAddress, settings.FromName);
        foreach (var recipient in to)
            message.To.Add(recipient.Trim());

        message.Subject = subject;
        message.Body = htmlBody;
        message.IsBodyHtml = true;

        await client.SendMailAsync(message);
        _logger.LogInformation("E-posta gonderildi: {Subject} -> {Recipients}", subject, string.Join(", ", to));
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var settings = await GetSmtpSettingsAsync();
            using var client = CreateSmtpClient(settings);

            // Send a test NOOP to verify connection
            using var message = new MailMessage();
            message.From = new MailAddress(settings.FromAddress, settings.FromName);
            message.To.Add(settings.FromAddress);
            message.Subject = "SMTP Baglanti Testi";
            message.Body = "Bu bir test e-postasıdır. SMTP bağlantınız başarılı.";
            message.IsBodyHtml = false;

            await client.SendMailAsync(message);
            _logger.LogInformation("SMTP baglanti testi basarili");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP baglanti testi basarisiz");
            return false;
        }
    }

    private SmtpClient CreateSmtpClient(SmtpSettings settings)
    {
        var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        if (!string.IsNullOrEmpty(settings.Username))
        {
            client.Credentials = new NetworkCredential(settings.Username, settings.Password);
        }

        return client;
    }

    private async Task<SmtpSettings> GetSmtpSettingsAsync()
    {
        var settings = await _db.AppSettings
            .Where(s => s.Category == SettingKeys.CategorySmtp)
            .ToListAsync();

        var dict = settings.ToDictionary(s => s.Key, s => s.Value ?? "");

        var host = dict.GetValueOrDefault(SettingKeys.SmtpHost, "");
        var port = int.TryParse(dict.GetValueOrDefault(SettingKeys.SmtpPort, "587"), out var p) ? p : 587;
        var username = dict.GetValueOrDefault(SettingKeys.SmtpUsername, "");
        var password = dict.GetValueOrDefault(SettingKeys.SmtpPassword, "");
        var fromAddress = dict.GetValueOrDefault(SettingKeys.SmtpFromAddress, "");
        var fromName = dict.GetValueOrDefault(SettingKeys.SmtpFromName, "Orko Portal");
        var enableSsl = dict.GetValueOrDefault(SettingKeys.SmtpEnableSsl, "true").Equals("true", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(fromAddress))
            throw new InvalidOperationException("SMTP ayarlari yapilandirilmamis. Lutfen Ayarlar sayfasindan SMTP ayarlarini yapin.");

        return new SmtpSettings(host, port, username, password, fromAddress, fromName, enableSsl);
    }

    private record SmtpSettings(string Host, int Port, string Username, string Password, string FromAddress, string FromName, bool EnableSsl);
}
