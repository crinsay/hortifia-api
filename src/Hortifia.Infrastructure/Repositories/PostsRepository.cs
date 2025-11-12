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

    public void Delete(Post post)
    {
        dbContext.Posts.Remove(post);
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

    public async Task<PostLike?> GetUserPostLikeAsync(int postId, string userId)
    {
        var postLike = await dbContext.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId 
                                 && pl.UserId == userId);

        return postLike;
    }
}
