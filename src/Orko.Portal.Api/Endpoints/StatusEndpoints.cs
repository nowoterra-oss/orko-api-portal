using Orko.Portal.Api.Extensions;
using Orko.Portal.Application.Statuses;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Contracts.Statuses;

namespace Orko.Portal.Api.Endpoints;

public static class StatusEndpoints
{
    public static void MapStatusEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/declarations/{id:guid}/status").WithTags("Statu Yonetimi");

        // Gecerli statu gecisleri
        group.MapGet("/transitions", async (Guid id, UpdateStatusHandler handler) =>
        {
            var result = await handler.GetAllowedTransitionsAsync(id);
            return result == null
                ? Results.NotFound(ApiResponse<object>.Fail("Beyanname bulunamadi."))
                : Results.Ok(ApiResponse<AllowedTransitionsDto>.Ok(result));
        })
        .WithName("GetAllowedTransitions");

        // Statu guncelle (manuel)
        group.MapPut("/", async (Guid id, UpdateStatusDto dto, HttpContext context, UpdateStatusHandler handler) =>
        {
            try
            {
                var userName = context.GetUserName();
                var result = await handler.HandleAsync(id, dto, string.IsNullOrEmpty(userName) ? "dashboard-user" : userName);
                return Results.Ok(ApiResponse<StatusHistoryDto>.Ok(result, "Statu guncellendi."));
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("UpdateStatus");
    }
}
