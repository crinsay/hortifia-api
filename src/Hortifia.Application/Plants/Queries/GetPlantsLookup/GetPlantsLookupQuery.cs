using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Queries.GetPlantsLookup;

public class GetPlantsLookupQuery : IRequest<Result<IEnumerable<PlantLookupDto>>>
{
    public required int LastPlantId { get; init; }
}
