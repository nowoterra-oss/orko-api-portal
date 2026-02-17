namespace Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

public class EvrimArchiveRequest
{
    public string? DeclarationId { get; set; }
    public string FileName { get; set; } = default!;
    public string Base64Data { get; set; } = default!;
    public string? DocumentType { get; set; }
}
