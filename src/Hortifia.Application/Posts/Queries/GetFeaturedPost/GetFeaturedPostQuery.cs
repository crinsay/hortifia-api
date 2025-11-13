using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Posts.Queries.GetFeaturedPost;

public class GetFeaturedPostQuery : IRequest<Result<DetailedPostDto>>
{
    public uint DaysSpan { get; init; } = 14;
}
