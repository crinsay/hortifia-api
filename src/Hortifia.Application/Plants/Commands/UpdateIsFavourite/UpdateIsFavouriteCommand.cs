using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.UpdateIsFavourite;

public class UpdateIsFavouriteCommand : IRequest<Result>
{
    public required int PlantId { get; init; }
}
