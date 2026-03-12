namespace Orko.Portal.Domain.Interfaces;

public interface IEvrimApiClient
{
    Task<string> GetTokenAsync();
    Task<EvrimResponse> CreateImportDeclarationAsync(object request);
    Task<EvrimResponse> CreateExportDeclarationAsync(object request);
    Task<EvrimResponse> CreateStatusAsync(object request);
    Task<EvrimResponse> CreateWorkOrderAsync(object request);
    Task<EvrimResponse> SendWorkOrderAsync(object request);
    Task<EvrimResponse> SendWorkOrderArchiveAsync(object request);

    // Yeni Evrim API endpoint'leri
    Task<EvrimResponse> CreateNewExportDeclarationAsync(object request);
    Task<EvrimResponse> CreateNewImportDeclarationAsync(object request);
}

/// <summary>
/// Evrim ResponseObject2 - tum POST endpoint'lerin ortak response'u
/// </summary>
public class EvrimResponse
{
    public string? ReferansNo { get; set; }
    public bool Success { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? EvrimReferansNo { get; set; }
    public string? Log { get; set; }

    // Orko icin ek alanlar
    public string? DeclarationId => EvrimReferansNo;
    public string? Message => ExceptionMessage;
    public string? RawResponse { get; set; }
}
