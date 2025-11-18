using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Domain.Common;
using Hortifia.Application.Common.Helpers;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(ILogger<CreatePostCommandHandler> logger,
    IPostsRepository postsRepository,
    IBlobStorageService blobStorageService,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<CreatePostCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = Post.Create(
            title: request.Title,
            content: request.Content,
            hashtagsContent: request.Hashtags,
            authorId: currentUser.Id!);

        int newPostId = 0;
        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            newPostId = await postsRepository.CreateAsync(post); // Internally calls SaveChangesAsync.

            var postImg = request.Img;
            if (postImg is not null)
            {
                var postImgName = postImg.FileName;

                var blobNameResult = BlobHelper.GetBlobName<Post>(post.Id, postImgName);
                if (!blobNameResult.IsSuccess)
                {
                    logger.LogCritical("[{handler}] Couldn't get blob name. BlobHelper might not be up to date!!!", nameof(CreatePostCommandHandler));
                    return Result.Failure(blobNameResult.ErrorMessage!);
                }

                post.ImgBlobName = blobNameResult.Value;
                await unitOfWork.SaveChangesAsync();

                var fileExtension = Path.GetExtension(postImgName).ToLowerInvariant();
                using var stream = postImg.OpenReadStream();
                await blobStorageService.UploadBlobAsync(stream, post.ImgBlobName!, fileExtension);
            }

            return Result.Success();
        });

        logger.LogInformation("[{handler}] Succesfully created new post.", nameof(CreatePostCommandHandler));
        return Result<int>.Success(newPostId);
    }
}
