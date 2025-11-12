using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.DeletePlant;

public class DeletePlantCommand : IRequest<Result>
{
    public int PlantId { get; init; }
}
