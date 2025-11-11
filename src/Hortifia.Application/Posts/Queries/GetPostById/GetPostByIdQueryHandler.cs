using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Hortifia.Application.Posts.Queries.GetPostById;

//logger tez
public class GetPostByIdQueryHandler(IPostsRepository postsRepository,
    IAuthorizationService authorizationService,
    IBlobStorageService blobStorageService,
    IUserContext userContext) : IRequestHandler<GetPostByIdQuery, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await postsRepository.GetByIdAsync(request.PostId, needsTracking: false, includeHashtags: true);
        if (post is null)
        {
            return Result<PostDto>.Failure("Post not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, post, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<PostDto>.Failure("Post not found.");
        }

        var postDto = PostDto.CreateFromEntity(post);

        var postImgBlobName = post.ImgBlobName;
        if (postImgBlobName is not null)
        {
            postDto.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(postImgBlobName);
        }

        return Result<PostDto>.Success(postDto);
    }
}
