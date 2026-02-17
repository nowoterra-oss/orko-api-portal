namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

public class EvrimStatusRequest
{
    public string? DeclarationId { get; set; }
    public string Status { get; set; } = default!;
}
