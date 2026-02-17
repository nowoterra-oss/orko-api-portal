namespace Orko.Portal.Contracts.WorkOrders;

public class CreateWorkOrderDto
{
    public string? SelsilOrderId { get; set; }
    public string Type { get; set; } = default!; // "Import" veya "Export"
    public string? CustomerName { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}
