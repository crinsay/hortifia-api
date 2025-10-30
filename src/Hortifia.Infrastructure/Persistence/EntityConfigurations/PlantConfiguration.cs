using Hortifia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hortifia.Infrastructure.Persistence.EntityConfigurations;

internal class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(20);

        builder.Property(p => p.CommonName)
            .HasMaxLength(100);

        builder.Property(p => p.ImgBlobName)
            .HasMaxLength(512);
    }
}
