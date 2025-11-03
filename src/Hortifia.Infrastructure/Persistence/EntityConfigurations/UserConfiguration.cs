using Hortifia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hortifia.Infrastructure.Persistence.EntityConfigurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.Rooms)
            .WithOne()
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Plants)
            .WithOne()
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.Posts)
            .WithOne(p => p.Author)
            .HasForeignKey(fk => fk.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.PostLikes)
            .WithOne()
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsOne(u => u.Coordinates);

        builder.Property(u => u.Nickname)
            .HasMaxLength(20);
    }
}
