using Hortifia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hortifia.Infrastructure.Persistence.EntityConfigurations;

internal class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasMany(r => r.Plants)
            .WithOne(p => p.Room)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.Name)
            .HasMaxLength(20);
    }
}
