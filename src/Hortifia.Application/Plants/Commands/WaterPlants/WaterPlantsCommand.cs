using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.WaterPlants;

public class WaterPlantsCommand : IRequest<Result>
{
    public List<int> PlantIds { get; set; } = [];
}