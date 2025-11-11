using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Posts.Queries.GetPostById;

public class GetPostByIdQuery : IRequest<Result<PostDto>>
{
    public required int PostId { get; init; }
}
