using Hortifia.Application.Common.Helpers;
using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(ILogger<CreatePostCommandHandler> logger,
    IPostsRepository postsRepository,
    IBlobStorageService blobStorageService,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<CreatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var post = Post.Create(
            title: request.Title,
            content: request.Content,
            hashtagsContent: request.Hashtags,
            authorId: currentUser.Id!);

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            int newPostId = await postsRepository.CreateAsync(post);

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

        var postDto = PostDto.CreateFromEntity(post);
        if (post.ImgBlobName is not null)
        {
            postDto.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(post.ImgBlobName);
        }
        return Result<PostDto>.Success(postDto);
    }
}
