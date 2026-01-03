using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Queries.GetFeaturedPost;

public class GetFeaturedPostQueryHandler(ILogger<GetFeaturedPostQueryHandler> logger,
    IPostsRepository postsRepository,
    IBlobStorageService blobStorageService,
    IUserContext userContext) : IRequestHandler<GetFeaturedPostQuery, Result<DetailedPostDto>>
{
    public async Task<Result<DetailedPostDto>> Handle(GetFeaturedPostQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = await postsRepository.GetFeaturedAsync(request.DaysSpan, currentUser.Id!);

        var postImgBlobName = post.ImgUrl;
        if (postImgBlobName is not null)
        {
            post.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(postImgBlobName);
        }

        logger.LogInformation("[{handler}] Succesfully fetched featured post (id = {postId}).", nameof(GetFeaturedPostQueryHandler), post.Id);
        return Result<DetailedPostDto>.Success(post);
    }
}
