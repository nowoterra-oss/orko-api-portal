using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Orko.Portal.Api.Middleware;

public class JwtAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;

    // Public paths that don't require authentication
    private static readonly string[] PublicPaths =
    [
        "/api/auth/login",
        "/api/auth/setup",
        "/api/auth/setup-status",
        "/health",
        "/swagger",
        "/hangfire"
    ];

    public JwtAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _secret = configuration["Jwt:Secret"] ?? throw new ArgumentException("Jwt:Secret is required");
        _issuer = configuration["Jwt:Issuer"] ?? "OrkoPortal";
        _audience = configuration["Jwt:Audience"] ?? "OrkoPortalFrontend";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // Skip non-API paths
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Skip public paths
        foreach (var publicPath in PublicPaths)
        {
            if (path.StartsWith(publicPath, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
        }

        // POST /api/work-orders uses API key auth (Selsil integration) - skip JWT
        if (path.StartsWith("/api/work-orders", StringComparison.OrdinalIgnoreCase) &&
            context.Request.Method == "POST")
        {
            await _next(context);
            return;
        }

        // Extract and validate JWT token
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Kimlik doğrulama gerekli." });
            return;
        }

        var token = authHeader["Bearer ".Length..];

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            context.User = principal;

            await _next(context);
        }
        catch (SecurityTokenException)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Geçersiz veya süresi dolmuş token." });
        }
    }
}
