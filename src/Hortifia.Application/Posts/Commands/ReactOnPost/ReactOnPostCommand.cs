using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Posts.Commands.ReactOnPost;

public class ReactOnPostCommand : IRequest<Result>
{
    public required int PostId { get; init; }
}
