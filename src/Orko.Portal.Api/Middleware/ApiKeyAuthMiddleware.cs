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
        // Sadece /api/work-orders POST icin API Key kontrolu (Selsil)
        if (context.Request.Path.StartsWithSegments("/api/work-orders") &&
            context.Request.Method == "POST")
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
