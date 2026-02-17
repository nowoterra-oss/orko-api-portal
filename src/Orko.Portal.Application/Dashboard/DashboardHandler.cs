using Microsoft.EntityFrameworkCore;
using Orko.Portal.Contracts.Dashboard;
using Orko.Portal.Domain.Enums;
using Orko.Portal.Infrastructure.Persistence;

namespace Orko.Portal.Application.Dashboard;

public class DashboardHandler
{
    private readonly PortalDbContext _db;

    public DashboardHandler(PortalDbContext db) => _db = db;

    public async Task<DashboardSummaryDto> HandleAsync()
    {
        var today = DateTime.UtcNow.Date;

        var totalWorkOrders = await _db.WorkOrders.CountAsync();

        var pendingDeclarations = await _db.Declarations
            .CountAsync(d => !d.SentToEvrim);

        var sentToEvrim = await _db.Declarations
            .CountAsync(d => d.SentToEvrim);

        var completedToday = await _db.WorkOrders
            .CountAsync(w => w.Status == WorkOrderStatus.Kapandi && w.UpdatedAt.Date == today);

        // Statu dagilimi
        var statusGroups = await _db.WorkOrders
            .GroupBy(w => w.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var statusDistribution = statusGroups
            .ToDictionary(g => g.Status, g => g.Count);

        // Son aktiviteler
        var recentActivities = await _db.StatusHistories
            .Include(s => s.WorkOrder)
            .OrderByDescending(s => s.CreatedAt)
            .Take(10)
            .Select(s => new RecentActivityDto
            {
                FileNumber = s.WorkOrder.FileNumber,
                Action = $"{s.FromStatus} -> {s.ToStatus}",
                User = s.ChangedBy,
                Timestamp = s.CreatedAt
            })
            .ToListAsync();

        return new DashboardSummaryDto
        {
            TotalWorkOrders = totalWorkOrders,
            PendingDeclarations = pendingDeclarations,
            SentToEvrim = sentToEvrim,
            CompletedToday = completedToday,
            StatusDistribution = statusDistribution,
            RecentActivities = recentActivities
        };
    }
}
