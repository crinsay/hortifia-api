using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Queries.GetFeaturedPost;

public class GetFeaturedPostQueryHandler(ILogger<GetFeaturedPostQueryHandler> logger,
    IPostsRepository postsRepository,
    IBlobStorageService blobStorageService) : IRequestHandler<GetFeaturedPostQuery, Result<DetailedPostDto>>
{
    public async Task<Result<DetailedPostDto>> Handle(GetFeaturedPostQuery request, CancellationToken cancellationToken)
    {
        var post = await postsRepository.GetFeaturedAsync(request.DaysSpan);

        var postImgBlobName = post.ImgUrl;
        if (postImgBlobName is not null)
        {
            post.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(postImgBlobName);
        }

        logger.LogInformation("[{handler}] Succesfully fetched featured post (id = {postId}).", nameof(GetFeaturedPostQueryHandler), post.Id);
        return Result<DetailedPostDto>.Success(post);
    }
}
