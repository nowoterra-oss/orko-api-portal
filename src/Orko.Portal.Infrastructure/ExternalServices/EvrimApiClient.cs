using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

namespace Orko.Portal.Infrastructure.ExternalServices;

public class EvrimApiClient : IEvrimApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<EvrimApiClient> _logger;

    public EvrimApiClient(HttpClient http, ILogger<EvrimApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<EvrimResponse> CreateExportDeclarationAsync(object request)
    {
        return await SendAsync("Declaration/CreateExportDeclaration", request, "Export");
    }

    public async Task<EvrimResponse> CreateImportDeclarationAsync(object request)
    {
        return await SendAsync("Declaration/CreateImportDeclaration", request, "Import");
    }

    public async Task<EvrimResponse> UploadArchiveAsync(object request)
    {
        return await SendAsync("Archive", request, "Archive");
    }

    public async Task<EvrimResponse> UpdateStatusAsync(object request)
    {
        return await SendAsync("Status", request, "Status");
    }

    private async Task<EvrimResponse> SendAsync(string endpoint, object request, string operationType)
    {
        var sw = Stopwatch.StartNew();
        var requestJson = JsonSerializer.Serialize(request);

        try
        {
            _logger.LogInformation(
                "Evrim {Operation} baslatildi: {Endpoint} | Request: {Request}",
                operationType, endpoint, requestJson);

            var response = await _http.PostAsJsonAsync(endpoint, request);
            var responseBody = await response.Content.ReadAsStringAsync();
            sw.Stop();

            _logger.LogInformation(
                "Evrim {Operation}: {StatusCode} | {Duration}ms | Response: {Response}",
                operationType, (int)response.StatusCode, sw.ElapsedMilliseconds, responseBody);

            if (!response.IsSuccessStatusCode)
            {
                return new EvrimResponse
                {
                    Success = false,
                    Message = $"Evrim API hatasi: {response.StatusCode}",
                    RawResponse = responseBody
                };
            }

            var result = JsonSerializer.Deserialize<EvrimResponse>(responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new EvrimResponse
            {
                Success = true,
                RawResponse = responseBody
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex,
                "Evrim {Operation} hatasi: {Endpoint} | {Duration}ms | Hata: {Error}",
                operationType, endpoint, sw.ElapsedMilliseconds, ex.Message);

            return new EvrimResponse
            {
                Success = false,
                Message = $"Evrim baglanti hatasi: {ex.Message}"
            };
        }
    }
}
