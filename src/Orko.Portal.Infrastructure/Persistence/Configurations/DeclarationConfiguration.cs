using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orko.Portal.Domain.Entities;

namespace Orko.Portal.Infrastructure.Persistence.Configurations;

public class DeclarationConfiguration : IEntityTypeConfiguration<Declaration>
{
    public void Configure(EntityTypeBuilder<Declaration> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeclarationType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.EvrimDeclarationId)
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(100);

        builder.HasMany(x => x.Archives)
            .WithOne(x => x.Declaration)
            .HasForeignKey(x => x.DeclarationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
