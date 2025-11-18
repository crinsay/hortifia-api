using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Application.Posts.Queries.GetPostById;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Queries.GetPosts;

public class GetPostsQueryHandler(ILogger<GetPostsQueryHandler> logger, 
    IPostsRepository postsRepository,
    IBlobStorageService blobStorageService,
    IUserContext userContext) : IRequestHandler<GetPostsQuery, Result<IEnumerable<DetailedPostDto>>>
{
    public async Task<Result<IEnumerable<DetailedPostDto>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var postDtos = await postsRepository.GetMatchingAsync(
            category: request.Category,
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

        logger.LogInformation("[{handler}] Succesfully fetched chunk of posts for user with id = {userId}", nameof(GetPostsQueryHandler), currentUser.Id);
        return Result<IEnumerable<DetailedPostDto>>.Success(postDtos);
    }
}
