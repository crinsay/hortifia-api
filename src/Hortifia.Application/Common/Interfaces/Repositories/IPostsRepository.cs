using Hortifia.Application.Posts.Dtos;
using Hortifia.Application.Posts.Queries.GetPosts;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IPostsRepository
{
    Task<int> CreateAsync(Post post);
    void Delete(Post post);
    Task<Post?> GetByIdAsync(int postId, bool needsTracking = false, bool includeHashtags = false);
    Task<IEnumerable<DetailedPostDto>> GetMatchingAsync(PostCategory category, 
        int alreadyFetchedItemsCount, 
        int pageSize, 
        SortBy sortBy, 
        IEnumerable<string> hashtags, 
        string? searchPhrase,
        string? userId);
    Task<DetailedPostDto> GetFeaturedAsync(uint daysSpan, string userId);
    Task<PostLike?> GetUserPostLikeAsync(int postId, string userId);
    Task<IEnumerable<string>> GetBlobNamesByUserIdAsync(string userId);
}
