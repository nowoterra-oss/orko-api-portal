namespace Orko.Portal.Contracts.Declarations;

public class DeclarationSummaryDto
{
    public Guid Id { get; set; }
    public string DeclarationType { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? EvrimDeclarationId { get; set; }
    public bool SentToEvrim { get; set; }
    public DateTime? SentAt { get; set; }
    public int ArchiveCount { get; set; }
}
