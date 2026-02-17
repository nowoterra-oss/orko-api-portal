using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orko.Portal.Domain.Entities;

namespace Orko.Portal.Infrastructure.Persistence.Configurations;

public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
{
    public void Configure(EntityTypeBuilder<StatusHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FromStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.ToStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.ChangedBy)
            .HasMaxLength(100);

        builder.Property(x => x.Note)
            .HasMaxLength(500);
    }
}
