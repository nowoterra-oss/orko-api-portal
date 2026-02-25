namespace Orko.Portal.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(IEnumerable<string> to, string subject, string htmlBody);
    Task<bool> TestConnectionAsync();
}
