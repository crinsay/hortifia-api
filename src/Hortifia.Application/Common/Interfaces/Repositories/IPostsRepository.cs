using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IPostsRepository
{
    Task<int> CreateAsync(Post post);
    void Delete(Post post);
    Task<Post?> GetByIdAsync(int postId, bool needsTracking = false, bool includeHashtags = false);
    Task<PostLike?> GetUserPostLikeAsync(int postId, string userId);
}
