using Hortifia.Application.Common.Helpers;
using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Posts.Commands.CreatePost;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler(ILogger<UpdatePostCommandHandler> logger,
    IBlobStorageService blobStorageService,
    IAuthorizationService authorizationService,
    IPostsRepository postsRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<UpdatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = await postsRepository.GetByIdAsync(request.PostId, needsTracking: true, includeHashtags: true);
        if (post is null)
        {
            logger.LogInformation("[{handler}] Couldn't find post with id = {postId}", nameof(UpdatePostCommandHandler), request.PostId);
            return Result<PostDto>.Failure("Post not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, post, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogInformation("[{handler}] user with id = {userId} attempted to update someone's else post.", nameof(UpdatePostCommandHandler), currentUser.Id);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<PostDto>.Failure("Post not found.");
        }

        post.Update(
            title: request.Title,
            content: request.Content,
            hashtagsContent: request.Hashtags);

        var oldPostImgBlobName = post.ImgBlobName;
        var newPostImg = request.Img;
        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            if (newPostImg is null)
            {
                post.ImgBlobName = null;
                await unitOfWork.SaveChangesAsync();

                if (oldPostImgBlobName is not null)
                {
                    await blobStorageService.DeleteBlobAsync(oldPostImgBlobName);
                }
            }
            else
            {
                var newPostImgName = newPostImg.FileName;

                var blobNameResult = BlobHelper.GetBlobName<Post>(post.Id, newPostImgName);
                if (!blobNameResult.IsSuccess)
                {
                    logger.LogCritical("[{handler}] Couldn't get blob name. BlobHelper might not be up to date!!!", nameof(UpdatePostCommandHandler));
                    return Result.Failure(blobNameResult.ErrorMessage!);
                }

                post.ImgBlobName = blobNameResult.Value;
                await unitOfWork.SaveChangesAsync();

                var fileExtension = Path.GetExtension(newPostImgName).ToLowerInvariant();
                using var stream = newPostImg.OpenReadStream();
                if (oldPostImgBlobName is null)
                {
                    await blobStorageService.UploadBlobAsync(stream, post.ImgBlobName!, fileExtension);
                }
                else
                {
                    await blobStorageService.ReplaceBlobAsync(newBlobContent: stream, 
                        newBlobName: post.ImgBlobName!,
                        newBlobContentType: fileExtension, 
                        oldBlobName: oldPostImgBlobName);
                }
            }

            return Result.Success();
        });
        logger.LogInformation("[{handler}] Succesfully updated post with id = {postId}.", nameof(UpdatePostCommandHandler), request.PostId);

        var postDto = PostDto.CreateFromEntity(post);
        var postImgBlobName = post.ImgBlobName;
        if (postImgBlobName is not null)
        {
            postDto.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(postImgBlobName);
        }
        return Result<PostDto>.Success(postDto);
    }
}
