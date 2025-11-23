using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Queries.GetPlants;

public class GetPlantsQuery : IRequest<Result<IEnumerable<PlantListDto>>>
{
    public string? SearchPhrase { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool OnlyFavourites { get; init; } = false;
    public bool LimitToFour { get; init; } = false;

    public bool OnlyPlantsInNeed = false;
}
