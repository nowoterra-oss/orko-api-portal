namespace Orko.Portal.Contracts.WorkOrders;

public class WorkOrderListDto
{
    public Guid Id { get; set; }
    public string FileNumber { get; set; } = default!;
    public string? SelsilOrderId { get; set; }
    public string Type { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? CustomerName { get; set; }
    public bool HasDeclaration { get; set; }
    public bool SentToEvrim { get; set; }
    public DateTime CreatedAt { get; set; }
}
