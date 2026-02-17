using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orko.Portal.Domain.Entities;

namespace Orko.Portal.Infrastructure.Persistence.Configurations;

public class ArchiveConfiguration : IEntityTypeConfiguration<Archive>
{
    public void Configure(EntityTypeBuilder<Archive> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .HasMaxLength(100);

        builder.Property(x => x.DocumentType)
            .HasMaxLength(50);

        builder.Property(x => x.UploadedBy)
            .HasMaxLength(100);
    }
}
