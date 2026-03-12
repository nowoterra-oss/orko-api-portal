namespace Orko.Portal.Api.Middleware;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _apiKey = config["ApiKey"] ?? "default-dev-key";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // API Key kontrolu: Selsil ve Direct Declaration endpointleri
        var path = context.Request.Path;
        var isApiKeyRequired = context.Request.Method == "POST" && (
            path.StartsWithSegments("/api/work-orders") ||
            path.StartsWithSegments("/api/create_export_declaration") ||
            path.StartsWithSegments("/api/create_import_declaration"));

        if (isApiKeyRequired)
        {
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey) ||
                extractedApiKey != _apiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Gecersiz API Key." });
                return;
            }
        }

        await _next(context);
    }
}
