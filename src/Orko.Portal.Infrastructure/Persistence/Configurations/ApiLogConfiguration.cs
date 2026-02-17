using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orko.Portal.Domain.Entities;

namespace Orko.Portal.Infrastructure.Persistence.Configurations;

public class ApiLogConfiguration : IEntityTypeConfiguration<ApiLog>
{
    public void Configure(EntityTypeBuilder<ApiLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Direction)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Endpoint)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Method)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(x => x.CreatedAt);
    }
}
