using Orko.Portal.Domain.Enums;

namespace Orko.Portal.Domain.Entities;

public class Declaration
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public DeclarationType DeclarationType { get; set; }
    public string? DeclarationData { get; set; }
    public string? EvrimDeclarationId { get; set; }
    public string? EvrimResponse { get; set; }
    public bool SentToEvrim { get; set; }
    public DateTime? SentAt { get; set; }
    public WorkOrderStatus Status { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public WorkOrder WorkOrder { get; set; } = default!;
    public ICollection<Archive> Archives { get; set; } = new List<Archive>();
}
