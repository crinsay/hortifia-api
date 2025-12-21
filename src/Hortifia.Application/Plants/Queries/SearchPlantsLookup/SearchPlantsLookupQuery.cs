using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Queries.SearchPlantsLookup;

public class SearchPlantsLookupQuery : IRequest<Result<IEnumerable<PlantLookupDto>>>
{
    public required string SearchPhrase { get; init; }
}
