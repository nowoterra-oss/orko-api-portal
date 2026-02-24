using Orko.Portal.Api.Extensions;
using Orko.Portal.Application.Archives;
using Orko.Portal.Contracts.Archives;
using Orko.Portal.Contracts.Common;

namespace Orko.Portal.Api.Endpoints;

public static class ArchiveEndpoints
{
    public static void MapArchiveEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/declarations/{declarationId:guid}/archives").WithTags("Arsivler");

        // Arsiv listesi
        group.MapGet("/", async (Guid declarationId, UploadArchiveHandler handler) =>
        {
            var result = await handler.GetByDeclarationAsync(declarationId);
            return Results.Ok(ApiResponse<List<ArchiveDto>>.Ok(result));
        })
        .WithName("ListArchives");

        // Arsiv yukle
        group.MapPost("/", async (Guid declarationId, UploadArchiveDto dto, HttpContext context, UploadArchiveHandler handler) =>
        {
            try
            {
                var userName = context.GetUserName();
                var result = await handler.HandleAsync(declarationId, dto, string.IsNullOrEmpty(userName) ? "dashboard-user" : userName);
                return Results.Created($"/api/declarations/{declarationId}/archives/{result.Id}",
                    ApiResponse<ArchiveDto>.Ok(result, "Arsiv yuklendi."));
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("UploadArchive");

        // Arsivi Evrim'e gonder
        group.MapPost("/{archiveId:guid}/send", async (Guid declarationId, Guid archiveId, UploadArchiveHandler handler) =>
        {
            try
            {
                var result = await handler.SendToEvrimAsync(archiveId);
                return result
                    ? Results.Ok(ApiResponse<object>.Ok(new { }, "Arsiv Evrim'e gonderim kuyruguna eklendi."))
                    : Results.NotFound(ApiResponse<object>.Fail("Arsiv bulunamadi."));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("SendArchiveToEvrim");
    }
}
