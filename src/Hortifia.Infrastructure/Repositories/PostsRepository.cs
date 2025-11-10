using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;

namespace Hortifia.Infrastructure.Repositories;

internal class PostsRepository(HortifiaDbContext dbContext) : IPostsRepository
{
    public async Task<int> CreateAsync(Post post)
    {
        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync();

        return post.Id;
    }
}
