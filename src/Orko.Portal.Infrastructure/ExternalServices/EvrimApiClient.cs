using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orko.Portal.Domain.Interfaces;

namespace Orko.Portal.Infrastructure.ExternalServices;

public class EvrimApiClient : IEvrimApiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<EvrimApiClient> _logger;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public EvrimApiClient(HttpClient http, IConfiguration config, ILogger<EvrimApiClient> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// JWT Token al (POST /api/login)
    /// Token'i 55 dakika cache'le (tipik 60 dk ömür)
    /// </summary>
    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            return _cachedToken;

        var username = _config["Evrim:Username"];
        var password = _config["Evrim:Password"];

        var request = new HttpRequestMessage(HttpMethod.Post, "api/login");
        request.Headers.Add("username", username);
        request.Headers.Add("password", password);

        var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Evrim login basarisiz: {StatusCode} | {Body}", (int)response.StatusCode, body);
            throw new Exception($"Evrim login hatasi: {response.StatusCode}");
        }

        var result = JsonSerializer.Deserialize<JsonElement>(body);
        _cachedToken = result.GetProperty("token").GetString()!;
        _tokenExpiry = DateTime.UtcNow.AddMinutes(55);

        _logger.LogInformation("Evrim JWT token alindi, gecerlilik: {Expiry}", _tokenExpiry);
        return _cachedToken;
    }

    public async Task<EvrimResponse> CreateImportDeclarationAsync(object request)
        => await SendAsync("api/import", request, "Import-Create");

    public async Task<EvrimResponse> CreateExportDeclarationAsync(object request)
        => await SendAsync("api/export", request, "Export-Create");

    public async Task<EvrimResponse> CreateStatusAsync(object request)
        => await SendAsync("api/status", request, "Status-Create");

    public async Task<EvrimResponse> CreateWorkOrderAsync(object request)
        => await SendAsync("api/workorder", request, "WorkOrder-Create");

    public async Task<EvrimResponse> SendWorkOrderAsync(object request)
        => await SendAsync("api/sendworkorder", request, "SendWorkOrder");

    public async Task<EvrimResponse> SendWorkOrderArchiveAsync(object request)
        => await SendAsync("api/sendworkorderarchive", request, "SendWorkOrderArchive");

    // Geriye uyumluluk icin (eski interface metotlari)
    public async Task<EvrimResponse> UploadArchiveAsync(object request)
        => await SendWorkOrderArchiveAsync(request);

    public async Task<EvrimResponse> UpdateStatusAsync(object request)
        => await CreateStatusAsync(request);

    private async Task<EvrimResponse> SendAsync(string endpoint, object request, string operationType)
    {
        var sw = Stopwatch.StartNew();
        var requestJson = JsonSerializer.Serialize(request);

        try
        {
            // Token al ve header'a ekle
            var token = await GetTokenAsync();
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation(
                "Evrim {Operation}: {Endpoint} | Request: {Request}",
                operationType, endpoint, requestJson);

            var response = await _http.PostAsJsonAsync(endpoint, request);
            var responseBody = await response.Content.ReadAsStringAsync();
            sw.Stop();

            _logger.LogInformation(
                "Evrim {Operation}: {StatusCode} | {Duration}ms | Response: {Response}",
                operationType, (int)response.StatusCode, sw.ElapsedMilliseconds, responseBody);

            // 401 ise token'i temizle ve tekrar dene
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _cachedToken = null;
                _tokenExpiry = DateTime.MinValue;

                _logger.LogWarning("Evrim token expired, yeniden deniyor...");
                token = await GetTokenAsync();
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                response = await _http.PostAsJsonAsync(endpoint, request);
                responseBody = await response.Content.ReadAsStringAsync();
            }

            if (!response.IsSuccessStatusCode)
            {
                return new EvrimResponse
                {
                    Success = false,
                    ExceptionMessage = $"Evrim API hatasi: {response.StatusCode}",
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
                ExceptionMessage = $"Evrim baglanti hatasi: {ex.Message}"
            };
        }
    }
}
