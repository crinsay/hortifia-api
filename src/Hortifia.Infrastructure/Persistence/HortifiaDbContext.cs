using Hortifia.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hortifia.Infrastructure.Persistence;

internal class HortifiaDbContext(DbContextOptions<HortifiaDbContext> options) 
    : IdentityDbContext<User>(options)
{
    internal DbSet<Room> Rooms { get; set; }
    internal DbSet<Plant> Plants { get; set; }
    internal DbSet<Post> Posts { get; set; }
    internal DbSet<Hashtag> Hashtags { get; set; }
    internal DbSet<PostLike> PostLikes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(HortifiaDbContext).Assembly);
    }
}
