using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.WaterPlants;

public class WaterPlantsCommand : IRequest<Result<IEnumerable<WateredPlantDto>>>
{
    public List<int> PlantIds { get; init; } = [];
}