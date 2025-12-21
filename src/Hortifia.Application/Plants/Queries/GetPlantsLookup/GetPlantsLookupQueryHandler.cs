using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.GetPlantsLookup;

public class GetPlantsLookupQueryHandler(IPermapeopleApiService apiService,
    ILogger<GetPlantsLookupQuery> logger) : IRequestHandler<GetPlantsLookupQuery, Result<IEnumerable<PlantLookupDto>>>
{
    public async Task<Result<IEnumerable<PlantLookupDto>>> Handle(GetPlantsLookupQuery request, CancellationToken cancellationToken)
    {
        var plantsApi = await apiService.GetPlantsAsync(request.LastPlantId);

        if (plantsApi is null || !plantsApi.Any())
        {
            logger.LogWarning("No plants found in external API for lookup.");
            return Result<IEnumerable<PlantLookupDto>>.Failure("No plants found.");
        }

        return Result<IEnumerable<PlantLookupDto>>.Success(plantsApi);
    }
}