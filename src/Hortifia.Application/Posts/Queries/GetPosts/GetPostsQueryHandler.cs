using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Posts.Queries.GetPosts;

public class GetPostsQueryHandler(IPostsRepository postsRepository,
    IBlobStorageService blobStorageService,
    IUserContext userContext) : IRequestHandler<GetPostsQuery, Result<IEnumerable<DetailedPostDto>>>
{
    public async Task<Result<IEnumerable<DetailedPostDto>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var postDtos = await postsRepository.GetMatchingAsync(category: request.Category,
            alreadyFetchedItemsCount: request.AlreadyFetchedItemsCount,
            pageSize: request.PageSize,
            sortBy: request.SortBy,
            hashtags: request.Hashtags,
            searchPhrase: request.SearchPhrase,
            userId: currentUser.Id);

        foreach (var postDto in postDtos)
        { 
            var postDtoImgUrl = postDto.ImgUrl;
            if (!string.IsNullOrEmpty(postDtoImgUrl))
            {
                postDto.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(postDtoImgUrl);
            }
        }

        return Result<IEnumerable<DetailedPostDto>>.Success(postDtos);
    }
}
