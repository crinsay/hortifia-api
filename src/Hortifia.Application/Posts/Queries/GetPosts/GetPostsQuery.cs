using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Posts.Queries.GetPosts;

public enum PostCategory
{
    All = 0,
    Own = 1,
    Favourites = 2
}

public enum SortBy
{
    Recent = 0,
    Popular = 1
}

public class GetPostsQuery : IRequest<Result<IEnumerable<DetailedPostDto>>>
{
    public PostCategory Category { get; init; } = PostCategory.All;
    public required int AlreadyFetchedItemsCount { get; init; }
    public required int PageSize { get; init; } = 20;
    public SortBy SortBy { get; init; } = SortBy.Recent;
    public ICollection<string> Hashtags { get; init; } = [];
    public string? SearchPhrase { get; init; }
}
