namespace Orko.Portal.Contracts.Statuses;

public class StatusHistoryDto
{
    public Guid Id { get; set; }
    public string FromStatus { get; set; } = default!;
    public string ToStatus { get; set; } = default!;
    public string? ChangedBy { get; set; }
    public string? Note { get; set; }
    public bool EvrimNotified { get; set; }
    public DateTime CreatedAt { get; set; }
}
