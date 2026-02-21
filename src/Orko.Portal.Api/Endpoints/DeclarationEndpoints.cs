using Orko.Portal.Application.Declarations;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Contracts.Declarations;

namespace Orko.Portal.Api.Endpoints;

public static class DeclarationEndpoints
{
    public static void MapDeclarationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/declarations").WithTags("Beyannameler");

        // Beyanname detay
        group.MapGet("/{id:guid}", async (Guid id, GetDeclarationHandler handler) =>
        {
            var result = await handler.HandleAsync(id);
            return result == null
                ? Results.NotFound(ApiResponse<object>.Fail("Beyanname bulunamadi."))
                : Results.Ok(ApiResponse<DeclarationDetailDto>.Ok(result));
        })
        .WithName("GetDeclaration");

        // Beyanname guncelle (kullanici formu doldurur)
        group.MapPut("/{id:guid}", async (Guid id, UpdateDeclarationDto dto, UpdateDeclarationHandler handler) =>
        {
            try
            {
                var result = await handler.HandleAsync(id, dto);
                return result
                    ? Results.Ok(ApiResponse<object>.Ok(new { }, "Beyanname guncellendi."))
                    : Results.NotFound(ApiResponse<object>.Fail("Beyanname bulunamadi."));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("UpdateDeclaration");

        // Evrim'e gonder
        group.MapPost("/{id:guid}/send", async (Guid id, SendToEvrimHandler handler) =>
        {
            try
            {
                var result = await handler.HandleAsync(id);
                return result.Success
                    ? Results.Ok(ApiResponse<object>.Ok(
                        new { EvrimDeclarationId = result.DeclarationId },
                        "Evrim'e basariyla gonderildi."))
                    : Results.BadRequest(ApiResponse<object>.Fail(
                        $"Evrim gonderilemedi: {result.Message}"));
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("SendToEvrim");

        // Dosyadan yukle ve Evrim'e gonder
        group.MapPost("/{id:guid}/upload-and-send", async (Guid id, UploadAndSendDto dto, UploadAndSendHandler handler) =>
        {
            try
            {
                var result = await handler.HandleAsync(id, dto);
                return result.Success
                    ? Results.Ok(ApiResponse<object>.Ok(
                        new { EvrimDeclarationId = result.DeclarationId },
                        "Dosyadan yuklendi ve Evrim'e basariyla gonderildi."))
                    : Results.BadRequest(ApiResponse<object>.Fail(
                        $"Evrim gonderilemedi: {result.Message}"));
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("UploadAndSendToEvrim");
    }
}
