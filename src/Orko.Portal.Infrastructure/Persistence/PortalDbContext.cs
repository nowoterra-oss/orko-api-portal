using Microsoft.EntityFrameworkCore;
using Orko.Portal.Domain.Entities;

namespace Orko.Portal.Infrastructure.Persistence;

public class PortalDbContext : DbContext
{
    public PortalDbContext(DbContextOptions<PortalDbContext> options) : base(options) { }

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Declaration> Declarations => Set<Declaration>();
    public DbSet<Archive> Archives => Set<Archive>();
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ApiLog> ApiLogs => Set<ApiLog>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalDbContext).Assembly);
    }
}
