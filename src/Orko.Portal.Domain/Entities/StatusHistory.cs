using Orko.Portal.Domain.Enums;

namespace Orko.Portal.Domain.Entities;

public class StatusHistory
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public WorkOrderStatus FromStatus { get; set; }
    public WorkOrderStatus ToStatus { get; set; }
    public string? ChangedBy { get; set; }
    public string? Note { get; set; }
    public bool EvrimNotified { get; set; }
    public DateTime? EvrimNotifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public WorkOrder WorkOrder { get; set; } = default!;
}
