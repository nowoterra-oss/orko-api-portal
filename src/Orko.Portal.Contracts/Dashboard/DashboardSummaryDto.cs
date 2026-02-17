namespace Orko.Portal.Contracts.Dashboard;

public class DashboardSummaryDto
{
    public int TotalWorkOrders { get; set; }
    public int PendingDeclarations { get; set; }
    public int SentToEvrim { get; set; }
    public int CompletedToday { get; set; }
    public Dictionary<string, int> StatusDistribution { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class RecentActivityDto
{
    public string FileNumber { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string? User { get; set; }
    public DateTime Timestamp { get; set; }
}
