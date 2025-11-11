using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hortifia.Infrastructure.Repositories;

internal class PostsRepository(HortifiaDbContext dbContext) : IPostsRepository
{
    public async Task<int> CreateAsync(Post post)
    {
        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync();

        return post.Id;
    }

    public async Task<Post?> GetByIdAsync(int postId, bool needsTracking = false, bool includeHashtags = false)
    {
        var mainQuery = dbContext.Posts.AsQueryable();

        if (!needsTracking)
        {
            mainQuery = mainQuery.AsNoTracking();
        }

        if (includeHashtags)
        {
            mainQuery = mainQuery.Include(p => p.Hashtags);
        }

        var post = await mainQuery
            .FirstOrDefaultAsync(p => p.Id == postId);

        return post;
    }
}
