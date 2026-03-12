using Orko.Portal.Application.Declarations;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Infrastructure.ExternalServices.EvrimModels;

namespace Orko.Portal.Api.Endpoints;

public static class DirectDeclarationEndpoints
{
    public static void MapDirectDeclarationEndpoints(this WebApplication app)
    {
        // POST /api/create_export_declaration
        app.MapPost("/api/create_export_declaration", async (
            EvrimExportDeclarationRequest request,
            CreateDirectDeclarationHandler handler) =>
        {
            try
            {
                var result = await handler.HandleExportAsync(request);
                return Results.Ok(ApiResponse<object>.Ok(
                    new { result.FileNumber, result.WorkOrderId, result.DeclarationId },
                    "Ihracat is emri basariyla olusturuldu."));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("CreateExportDeclaration")
        .WithTags("Is Emirleri")
        .AllowAnonymous()
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(400);

        // POST /api/create_import_declaration
        app.MapPost("/api/create_import_declaration", async (
            EvrimCreateDeclarationRequest request,
            CreateDirectDeclarationHandler handler) =>
        {
            try
            {
                var result = await handler.HandleImportAsync(request);
                return Results.Ok(ApiResponse<object>.Ok(
                    new { result.FileNumber, result.WorkOrderId, result.DeclarationId },
                    "Ithalat is emri basariyla olusturuldu."));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("CreateImportDeclaration")
        .WithTags("Is Emirleri")
        .AllowAnonymous()
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(400);
    }
}
