using Orko.Portal.Domain.Enums;

namespace Orko.Portal.Domain.Entities;

public class WorkOrder
{
    public Guid Id { get; set; }
    public string FileNumber { get; set; } = default!;
    public string? SelsilOrderId { get; set; }
    public DeclarationType Type { get; set; }
    public WorkOrderStatus Status { get; set; }
    public string? SelsilPayload { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Declaration? Declaration { get; set; }
    public ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
}
