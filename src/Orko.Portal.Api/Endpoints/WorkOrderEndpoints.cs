using Orko.Portal.Application.WorkOrders;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Contracts.WorkOrders;

namespace Orko.Portal.Api.Endpoints;

public static class WorkOrderEndpoints
{
    public static void MapWorkOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/work-orders").WithTags("Is Emirleri");

        // Selsil is emri gonderir -> taslak beyanname olusur -> dosya no doner
        group.MapPost("/", async (CreateWorkOrderDto dto, CreateWorkOrderHandler handler) =>
        {
            try
            {
                var result = await handler.HandleAsync(dto);
                return Results.Created($"/api/work-orders/{result.FileNumber}",
                    ApiResponse<WorkOrderResponseDto>.Ok(result));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        })
        .WithName("CreateWorkOrder")
        .Produces<ApiResponse<WorkOrderResponseDto>>(201)
        .Produces<ApiResponse<object>>(400);

        // Is emri listesi (dashboard icin)
        group.MapGet("/", async (
            GetWorkOrdersHandler handler,
            int page = 1,
            int pageSize = 20,
            string? status = null,
            string? type = null,
            string? search = null) =>
        {
            var result = await handler.HandleAsync(page, pageSize, status, type, search);
            return Results.Ok(ApiResponse<PagedResult<WorkOrderListDto>>.Ok(result));
        })
        .WithName("ListWorkOrders");

        // Is emri detay (dosya numarasi ile)
        group.MapGet("/{fileNumber}", async (string fileNumber, GetWorkOrdersHandler handler) =>
        {
            var result = await handler.GetByFileNumberAsync(fileNumber);
            return result == null
                ? Results.NotFound(ApiResponse<object>.Fail("Is emri bulunamadi."))
                : Results.Ok(ApiResponse<WorkOrderResponseDto>.Ok(result));
        })
        .WithName("GetWorkOrder");
    }
}
