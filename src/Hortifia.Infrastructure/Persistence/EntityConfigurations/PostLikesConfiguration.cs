using Hortifia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hortifia.Infrastructure.Persistence.EntityConfigurations;

internal class PostLikes : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.HasKey(pk => new { pk.UserId, pk.PostId });

        builder.HasOne(pl => pl.Post)
            .WithMany()
            .HasForeignKey(fk => fk.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
