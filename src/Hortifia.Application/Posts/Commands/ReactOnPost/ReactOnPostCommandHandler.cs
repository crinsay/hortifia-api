using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;

namespace Hortifia.Application.Posts.Commands.ReactOnPost;

public class ReactOnPostCommandHandler(/*ILogger<ReactOnPostCommandHandler> logger,*/
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

        return Result.Success();
    }
}
