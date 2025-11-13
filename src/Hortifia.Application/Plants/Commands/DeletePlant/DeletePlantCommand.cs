using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.DeletePlant;

public class DeletePlantCommand : IRequest<Result>
{
    public required int PlantId { get; init; }
}
