using Hortifia.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Hortifia.Application.Posts.Commands.CreatePost;

public class CreatePostCommand : IRequest<Result<int>>
{
    public required string Title { get; init; }
    public required string Content { get; init; }
    public ICollection<string> Hashtags { get; init; } = [];
    public IFormFile? Img { get; init; }
}
