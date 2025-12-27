using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.GetPlantApiDataById;

public class GetPlantApiDataByIdQueryHandler(IPermapeopleApiService apiService,
    ILogger<GetPlantApiDataByIdQueryHandler> logger) : IRequestHandler<GetPlantApiDataByIdQuery, Result<PlantApiInfoDto>>
{
    public async Task<Result<PlantApiInfoDto>> Handle(GetPlantApiDataByIdQuery request, CancellationToken cancellationToken)
    {
        var plantApi = await apiService.GetPlantByIdAsync(request.PlantApiId);

        if (plantApi is null)
        {
            logger.LogWarning("Plant with PlantApiId {PlantApiId} not found in external API.", request.PlantApiId);
            return Result<PlantApiInfoDto>.Failure($"Plant with PlantApiId {request.PlantApiId} not found.");
        }

        var apiPlantData = plantApi.Data;

        var plantApiInfoDto = new PlantApiInfoDto
        {
            ScientificName = plantApi.ScientificName,
            Description = plantApi.Description,

            Family = apiPlantData?.FirstOrDefault
                (d => d.Key?.Equals("Family", StringComparison.OrdinalIgnoreCase) == true)?.Value,
            IsEdible = apiPlantData?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.Edible, StringComparison.OrdinalIgnoreCase) == true)?.Value
                ?.Equals("Yes", StringComparison.OrdinalIgnoreCase),
            Growth = apiPlantData?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.Growth, StringComparison.OrdinalIgnoreCase) == true)?.Value,
            WaterRequirement = apiPlantData?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.WaterRequirement, StringComparison.OrdinalIgnoreCase) == true)?.Value,
            LightRequirement = apiPlantData?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.LightRequirement, StringComparison.OrdinalIgnoreCase) == true)?.Value,
            SoilType = apiPlantData?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.SoilType, StringComparison.OrdinalIgnoreCase) == true)?.Value,
            EdibleParts = apiPlantData?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.EdibleParts, StringComparison.OrdinalIgnoreCase) == true)?.Value
        };


        return Result<PlantApiInfoDto>.Success(plantApiInfoDto);
    }
}