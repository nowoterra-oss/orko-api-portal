using Orko.Portal.Application.Declarations;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Domain.Interfaces;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

namespace Orko.Portal.Api.Endpoints;

public static class DirectDeclarationEndpoints
{
    public static void MapDirectDeclarationEndpoints(this WebApplication app)
    {
        // POST /api/create_export_declaration
        app.MapPost("/api/create_export_declaration", async (
            EvrimCreateDeclarationRequest request,
            CreateDirectDeclarationHandler handler) =>
        {
            try
            {
                var result = await handler.HandleAsync(request, DeclarationType.Export);
                return result.Success
                    ? Results.Ok(ApiResponse<object>.Ok(
                        new { result.ReferansNo, result.EvrimReferansNo },
                        "Ihracat beyannnamesi basariyla olusturuldu ve Evrim'e gonderildi."))
                    : Results.BadRequest(ApiResponse<object>.Fail(
                        $"Evrim gonderilemedi: {result.Message}"));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("CreateExportDeclaration")
        .WithTags("Is Emirleri")
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(400);

        // POST /api/create_import_declaration
        app.MapPost("/api/create_import_declaration", async (
            EvrimCreateDeclarationRequest request,
            CreateDirectDeclarationHandler handler) =>
        {
            try
            {
                var result = await handler.HandleAsync(request, DeclarationType.Import);
                return result.Success
                    ? Results.Ok(ApiResponse<object>.Ok(
                        new { result.ReferansNo, result.EvrimReferansNo },
                        "Ithalat beyannnamesi basariyla olusturuldu ve Evrim'e gonderildi."))
                    : Results.BadRequest(ApiResponse<object>.Fail(
                        $"Evrim gonderilemedi: {result.Message}"));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("CreateImportDeclaration")
        .WithTags("Is Emirleri")
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(400);
    }
}
