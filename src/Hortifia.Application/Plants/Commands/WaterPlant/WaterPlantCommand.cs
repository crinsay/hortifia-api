using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.WaterPlant;

public class WaterPlantCommand : IRequest<Result<WateredPlantDto>>
{
    public required int PlantId { get; init; }
}
