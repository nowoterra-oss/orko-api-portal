namespace Orko.Portal.Domain.Interfaces;

public interface IEvrimApiClient
{
    Task<EvrimResponse> CreateExportDeclarationAsync(object request);
    Task<EvrimResponse> CreateImportDeclarationAsync(object request);
    Task<EvrimResponse> UploadArchiveAsync(object request);
    Task<EvrimResponse> UpdateStatusAsync(object request);
}

public class EvrimResponse
{
    public bool Success { get; set; }
    public string? DeclarationId { get; set; }
    public string? Message { get; set; }
    public string? RawResponse { get; set; }
}
