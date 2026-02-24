using Orko.Portal.Api.Extensions;
using Orko.Portal.Application.Auth;
using Orko.Portal.Contracts.Auth;
using Orko.Portal.Contracts.Common;

namespace Orko.Portal.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Kimlik Dogrulama");

        // POST /api/auth/login
        group.MapPost("/login", async (LoginDto dto, AuthHandler handler) =>
        {
            var result = await handler.LoginAsync(dto);
            if (result == null)
                return Results.Unauthorized();

            return Results.Ok(ApiResponse<LoginResponseDto>.Ok(result, "Giriş başarılı."));
        })
        .WithName("Login")
        .AllowAnonymous();

        // GET /api/auth/setup-status
        group.MapGet("/setup-status", async (AuthHandler handler) =>
        {
            var result = await handler.GetSetupStatusAsync();
            return Results.Ok(ApiResponse<SetupStatusDto>.Ok(result));
        })
        .WithName("GetSetupStatus")
        .AllowAnonymous();

        // POST /api/auth/setup
        group.MapPost("/setup", async (SetupDto dto, AuthHandler handler) =>
        {
            var result = await handler.SetupAsync(dto);
            if (result == null)
                return Results.BadRequest(ApiResponse<object>.Fail("Kurulum zaten tamamlanmış."));

            return Results.Ok(ApiResponse<LoginResponseDto>.Ok(result, "Kurulum tamamlandı."));
        })
        .WithName("Setup")
        .AllowAnonymous();

        // GET /api/auth/me
        group.MapGet("/me", async (HttpContext context, AuthHandler handler) =>
        {
            var userId = context.GetUserId();
            if (userId == Guid.Empty)
                return Results.Unauthorized();

            var result = await handler.GetCurrentUserAsync(userId);
            if (result == null)
                return Results.Unauthorized();

            return Results.Ok(ApiResponse<UserInfoDto>.Ok(result));
        })
        .WithName("GetCurrentUser");
    }
}
