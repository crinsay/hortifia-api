using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Common.Types;
using Hortifia.Application.Posts.Commands.UpdatePost;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Application.Posts.Queries.GetFeaturedPost;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler(ILogger<GetPostByIdQueryHandler> logger, 
    IPostsRepository postsRepository,
    IAuthorizationService authorizationService,
    IBlobStorageService blobStorageService,
    IUserContext userContext) : IRequestHandler<GetPostByIdQuery, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = await postsRepository.GetByIdAsync(request.PostId, needsTracking: false, includeHashtags: true);
        if (post is null)
        {
            logger.LogInformation("[{handler}] Couldn't find post with id = {postId}", nameof(GetPostByIdQueryHandler), request.PostId);
            return Result<PostDto>.Failure("Post not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, post, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogInformation("[{handler}] user with id = {userId} attempted to access someone's else post.", nameof(GetPostByIdQueryHandler), currentUser.Id);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<PostDto>.Failure("Post not found.");
        }

        var postDto = PostDto.CreateFromEntity(post);

        var postImgBlobName = post.ImgBlobName;
        if (postImgBlobName is not null)
        {
            postDto.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(postImgBlobName);
        }

        logger.LogInformation("[{handler}] Succesfully fetched post with id = {postId}.", nameof(GetPostByIdQueryHandler), post.Id);
        return Result<PostDto>.Success(postDto);
    }
}
