using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Commands.ReactOnPost;

public class ReactOnPostCommandHandler(ILogger<ReactOnPostCommandHandler> logger,
    IPostsRepository postsRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<ReactOnPostCommand, Result>
{
    public async Task<Result> Handle(ReactOnPostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = await postsRepository.GetByIdAsync(request.PostId, needsTracking: true, includeHashtags: false);
        if (post is null)
        {
            logger.LogInformation("[{handler}] Couldn't find post with id = {postId}", nameof(ReactOnPostCommandHandler), request.PostId);
            return Result.Failure("Post not found.");
        }

        var currentUserId = currentUser.Id!;
        var postLike = await postsRepository.GetUserPostLikeAsync(post.Id, currentUserId);
        if (postLike is null)
        {
            post.Like(currentUserId);
        }
        else
        {
            post.Dislike(postLike);
        }

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("[{handler}] Succesfully reacted on post with id = {postId}.", nameof(ReactOnPostCommandHandler), request.PostId);
        return Result.Success();
    }
}
