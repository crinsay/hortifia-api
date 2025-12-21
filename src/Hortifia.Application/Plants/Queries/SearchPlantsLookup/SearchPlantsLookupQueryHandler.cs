using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.SearchPlantsLookup;

public class SearchPlantsLookupQueryHandler(IPermapeopleApiService apiService,
    ILogger<SearchPlantsLookupQuery> logger) : IRequestHandler<SearchPlantsLookupQuery, Result<IEnumerable<PlantLookupDto>>>
{
    public async Task<Result<IEnumerable<PlantLookupDto>>> Handle(SearchPlantsLookupQuery request, CancellationToken cancellationToken)
    {
        var plantsApi = await apiService.SearchPlantsAsync(request.SearchPhrase);

        if (plantsApi is null || !plantsApi.Any())
        {
            logger.LogWarning("No plants found in external API for lookup with search phrase: {SearchPhrase}", request.SearchPhrase);
            return Result<IEnumerable<PlantLookupDto>>.Failure("No plants found.");
        }

        return Result<IEnumerable<PlantLookupDto>>.Success(plantsApi);
    }
}
