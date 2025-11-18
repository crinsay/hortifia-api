using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Commands.DeletePost;

public class DeletePostCommandHandler(ILogger<DeletePostCommandHandler> logger,
    IPostsRepository postsRepository,
    IUnitOfWork unitOfWork,
    IBlobStorageService blobStorageService,
    IAuthorizationService authorizationService,
    IUserContext userContext) : IRequestHandler<DeletePostCommand, Result>
{
    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = await postsRepository.GetByIdAsync(request.PostId, needsTracking: true, includeHashtags: false);
        if (post is null)
        {
            logger.LogInformation("[{handler}] Couldn't find post with id = {postId}", nameof(DeletePostCommandHandler), request.PostId);
            return Result.Failure("Post not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, post, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogInformation("[{handler}] user with id = {userId} attempted to delete someone's else post.", nameof(DeletePostCommandHandler), currentUser.Id);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result.Failure("Post not found.");
        }

        postsRepository.Delete(post);

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await unitOfWork.SaveChangesAsync();

            var postImgBlobName = post.ImgBlobName;
            if (postImgBlobName is not null)
            {
                await blobStorageService.DeleteBlobAsync(postImgBlobName);
            }
        });

        logger.LogInformation("[{handler}] Succesfully deleted post with id = {postId}.", nameof(DeletePostCommandHandler), request.PostId);
        return Result.Success();
    }
}
