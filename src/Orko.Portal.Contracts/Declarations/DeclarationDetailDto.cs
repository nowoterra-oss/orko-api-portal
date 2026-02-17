using Orko.Portal.Contracts.Archives;
using Orko.Portal.Contracts.Statuses;

namespace Orko.Portal.Contracts.Declarations;

public class DeclarationDetailDto
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public string FileNumber { get; set; } = default!;
    public string DeclarationType { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? DeclarationData { get; set; }
    public string? EvrimDeclarationId { get; set; }
    public string? EvrimResponse { get; set; }
    public bool SentToEvrim { get; set; }
    public DateTime? SentAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ArchiveDto> Archives { get; set; } = new();
    public List<StatusHistoryDto> StatusHistory { get; set; } = new();
}
