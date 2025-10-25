using Hortifia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hortifia.Infrastructure.Persistence.EntityConfigurations;

internal class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasMany(p => p.Hashtags)
            .WithOne()
            .HasForeignKey(p => p.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Title)
            .HasMaxLength(100);

        builder.Property(p => p.Content)
            .HasMaxLength(4096);

        builder.Property(p => p.ImgBlobName)
            .HasMaxLength(512);
    }
}
