using Orko.Portal.Application.Dashboard;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Contracts.Dashboard;

namespace Orko.Portal.Api.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard");

        group.MapGet("/summary", async (DashboardHandler handler) =>
        {
            var result = await handler.HandleAsync();
            return Results.Ok(ApiResponse<DashboardSummaryDto>.Ok(result));
        })
        .WithName("GetDashboardSummary");
    }
}
