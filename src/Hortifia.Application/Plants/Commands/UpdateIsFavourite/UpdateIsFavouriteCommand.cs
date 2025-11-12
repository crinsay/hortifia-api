using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.UpdateIsFavourite;

public class UpdateIsFavouriteCommand : IRequest<Result>
{
    public int Id { get; init; }
}
