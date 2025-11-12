using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Domain.Common;
using Hortifia.Domain.Common.Helpers;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Hortifia.Application.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler(/*ILogger<UpdatePostCommandHandler> logger,*/
    IBlobStorageService blobStorageService,
    IAuthorizationService authorizationService,
    IPostsRepository postsRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<UpdatePostCommand, Result>
{
    public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = await postsRepository.GetByIdAsync(request.PostId, needsTracking: true, includeHashtags: true);
        if (post is null)
        {
            return Result.Failure("Post not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, post, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result.Failure("Post not found.");
        }

        post.Update(title: request.Title,
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

        return Result.Success();
    }
}
