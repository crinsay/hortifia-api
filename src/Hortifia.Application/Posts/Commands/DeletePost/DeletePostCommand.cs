using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Posts.Commands.DeletePost;

public class DeletePostCommand : IRequest<Result>
{
    public required int PostId { get; init; }
}
