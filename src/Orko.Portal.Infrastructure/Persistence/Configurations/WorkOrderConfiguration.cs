using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orko.Portal.Domain.Entities;

namespace Orko.Portal.Infrastructure.Persistence.Configurations;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.FileNumber)
            .IsUnique();

        builder.Property(x => x.SelsilOrderId)
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.Declaration)
            .WithOne(x => x.WorkOrder)
            .HasForeignKey<Declaration>(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StatusHistories)
            .WithOne(x => x.WorkOrder)
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
