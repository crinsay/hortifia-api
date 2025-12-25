using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Application.Posts.Queries.GetPosts;
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

    public async Task<IEnumerable<DetailedPostDto>> GetMatchingAsync(PostCategory category, 
        int alreadyFetchedItemsCount, 
        int pageSize, 
        SortBy sortBy, 
        IEnumerable<string> hashtags, 
        string? searchPhrase,
        string? userId)
    {
        var searchPhraseLower = searchPhrase?.ToLower().Trim();
        var hashtagsLower = hashtags.Select(hc => hc.ToLower().Trim());

        var mainQuery = dbContext.Posts
            .Where(p => searchPhraseLower == null
                   || p.Title.ToLower().Contains(searchPhraseLower)
                   || p.Content.ToLower().Contains(searchPhraseLower))
            .Where(p => !hashtagsLower.Any()
                   || p.Hashtags.Any(h => hashtagsLower.Contains(h.Content.ToLower())));

        if (category == PostCategory.Own)
        {
            mainQuery = mainQuery
                .Where(p => p.OwnerId == userId);
        }

        if (category == PostCategory.Favourites)
        {
            mainQuery = mainQuery
                .Where(p => p.PostLikes.Any(pl => pl.UserId == userId));
        }

        mainQuery = sortBy switch
        {
            SortBy.Recent => mainQuery
                .OrderByDescending(p => p.Hashtags.Count(h => hashtagsLower.Contains(h.Content.ToLower())))
                .ThenByDescending(p => p.CreateDate)
                .ThenByDescending(p => p.Id),
            SortBy.Popular => mainQuery
                .OrderByDescending(p => p.Hashtags.Count(h => hashtagsLower.Contains(h.Content.ToLower())))
                .ThenByDescending(p => p.PostLikes.Count())
                .ThenByDescending(p => p.CreateDate)
                .ThenByDescending(p => p.Id),
            _ => mainQuery
                .OrderByDescending(p => p.Hashtags.Count(h => hashtagsLower.Contains(h.Content.ToLower())))
                .ThenByDescending(p => p.CreateDate)
                .ThenByDescending(p => p.Id)
        };

        var posts = await mainQuery
            .Skip(alreadyFetchedItemsCount)
            .Take(pageSize)
            .Select(p => new DetailedPostDto
            {
                Id = p.Id,
                Title = p.Title,
                CreateDate = p.CreateDate,
                Content = p.Content,
                ImgUrl = p.ImgBlobName, // It will be replaced by generated sas url if not null in app layer.
                LikesNumber = p.PostLikes.Count(),
                Hashtags = p.Hashtags.Select(h => h.Content),
                Author = p.Author.Nickname,
                IsLiked = p.PostLikes.Any(pl => pl.UserId == userId)
            })
            .ToListAsync();

        return posts;
    }

    public async Task<DetailedPostDto> GetFeaturedAsync(uint daysSpan)
    {   
        var post = await dbContext.Posts
            .OrderByDescending(p => p.PostLikes.Count(pl => EF.Functions.DateDiffDay(pl.LikedAt, DateTime.UtcNow) <= daysSpan))
            .ThenByDescending(p => p.CreateDate)
            .ThenByDescending(p => p.Id)
            .Select(p => new DetailedPostDto
            {
                Id = p.Id,
                Title = p.Title,
                CreateDate = p.CreateDate,
                Content = p.Content,
                ImgUrl = p.ImgBlobName, // It will be replaced by generated sas url if not null in app layer.
                LikesNumber = p.PostLikes.Count(),
                Hashtags = p.Hashtags.Select(h => h.Content),
                Author = p.Author.Nickname,
            })
            .FirstAsync();

        return post;
    }

    public async Task<PostLike?> GetUserPostLikeAsync(int postId, string userId)
    {
        var postLike = await dbContext.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId 
                                 && pl.UserId == userId);

        return postLike;
    }

    public async Task<IEnumerable<string>> GetBlobNamesByUserIdAsync(string userId)
    {
        var blobNames = await dbContext.Posts
            .Where(p => p.OwnerId == userId
                   && p.ImgBlobName != null)
            .Select(p => p.ImgBlobName!)
            .ToListAsync();

        return blobNames;
    }
}
