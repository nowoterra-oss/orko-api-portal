using Orko.Portal.Contracts.Declarations;

namespace Orko.Portal.Contracts.WorkOrders;

public class WorkOrderResponseDto
{
    public Guid Id { get; set; }
    public string FileNumber { get; set; } = default!;
    public string? SelsilOrderId { get; set; }
    public string Type { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DeclarationSummaryDto? Declaration { get; set; }
}
