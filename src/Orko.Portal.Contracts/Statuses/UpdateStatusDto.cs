namespace Orko.Portal.Contracts.Statuses;

public class UpdateStatusDto
{
    public string NewStatus { get; set; } = default!;
    public string? Note { get; set; }
}
