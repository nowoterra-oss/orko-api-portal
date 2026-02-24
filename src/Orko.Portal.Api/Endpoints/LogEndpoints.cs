using Microsoft.EntityFrameworkCore;
using Orko.Portal.Contracts.Common;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Api.Endpoints;

public static class LogEndpoints
{
    public static void MapLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/logs").WithTags("Loglar");

        group.MapGet("/", async (
            PortalDbContext db,
            int page = 1,
            int pageSize = 30) =>
        {
            var query = db.ApiLogs.OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.Id,
                    l.Direction,
                    l.Endpoint,
                    l.Method,
                    l.RequestBody,
                    l.ResponseBody,
                    l.StatusCode,
                    l.DurationMs,
                    l.CreatedAt,
                    l.UserId,
                    l.UserName,
                    l.UserEmail,
                    l.UserRole
                })
                .ToListAsync();

            var result = new
            {
                items,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Results.Ok(ApiResponse<object>.Ok(result));
        })
        .WithName("ListLogs");
    }
}
