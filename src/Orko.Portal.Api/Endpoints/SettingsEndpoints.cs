using Orko.Portal.Api.Extensions;
using Orko.Portal.Application.Settings;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Contracts.Settings;
using Orko.Portal.Domain.Constants;

namespace Orko.Portal.Api.Endpoints;

public static class SettingsEndpoints
{
    public static void MapSettingsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/settings").WithTags("Ayarlar");

        // GET /api/settings
        group.MapGet("/", async (HttpContext context, SettingsHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            var result = await handler.GetAllAsync();
            return Results.Ok(ApiResponse<List<AppSettingDto>>.Ok(result));
        })
        .WithName("GetAllSettings");

        // GET /api/settings/{category}
        group.MapGet("/{category}", async (string category, HttpContext context, SettingsHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            var result = await handler.GetByCategoryAsync(category);
            return Results.Ok(ApiResponse<List<AppSettingDto>>.Ok(result));
        })
        .WithName("GetSettingsByCategory");

        // PUT /api/settings
        group.MapPut("/", async (HttpContext context, UpdateSettingsDto dto, SettingsHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            var updatedBy = context.GetUserName();
            await handler.UpdateAsync(dto, updatedBy);
            return Results.Ok(ApiResponse<object>.Ok(new { }, "Ayarlar guncellendi."));
        })
        .WithName("UpdateSettings");

        // POST /api/settings/smtp-test
        group.MapPost("/smtp-test", async (HttpContext context, SettingsHandler handler) =>
        {
            if (!context.IsInRole(UserRoles.Admin))
                return Results.Forbid();

            var result = await handler.TestSmtpAsync();
            return result.Success
                ? Results.Ok(ApiResponse<SmtpTestResultDto>.Ok(result))
                : Results.Ok(ApiResponse<SmtpTestResultDto>.Ok(result));
        })
        .WithName("TestSmtp");
    }
}
