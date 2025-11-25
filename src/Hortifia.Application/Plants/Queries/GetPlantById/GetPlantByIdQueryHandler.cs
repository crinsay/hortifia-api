using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.GetPlantById;

public class GetPlantByIdQueryHandler(IPlantsRepository plantsRepository,
    ILogger<GetPlantByIdQueryHandler> logger,
    IUserContext userContext,
    IMediator mediator,
    IBlobStorageService blobStorageService) : IRequestHandler<GetPlantByIdQuery, Result<PlantDto>>
{
    public async Task<Result<PlantDto>> Handle(GetPlantByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var plant = await plantsRepository.GetDtoByIdAsync(request.PlantId);

        if (plant is null) 
        {
            logger.LogInformation("Plant with ID {PlantId} not found for user {UserId}.", request.PlantId, currentUser.Id);
            return Result<PlantDto>.Failure("Plant not found.");
        }

        if (plant.Room is null || plant.Room.UserId != currentUser.Id)
        {
            logger.LogWarning("User {UserId} attempted to access plant {PlantId} which they do not own.", currentUser.Id, request.PlantId);
            return Result<PlantDto>.Failure("Room not found.");
        }

        var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = plant.PlantApiId }, cancellationToken);

        if (!apiPlantResult.IsSuccess || apiPlantResult.Value is null)
        {
            logger.LogWarning("Failed to retrieve API data for plant {PlantId}.", request.PlantId);
            return Result<PlantDto>.Failure("Failed to retrieve plant API data.");
        }

        var apiPlantData = apiPlantResult.Value.Data;

        var plantApiInfoDto = new PlantApiInfoDto
        {
            ScientificName = apiPlantResult.Value.ScientificName,
            Description = apiPlantResult.Value.Description,

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

        plant.PlantApiInfo = plantApiInfoDto;

        var imgBlobName = plant.ImgUrl;
        if (imgBlobName is not null)
        {
            plant.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(imgBlobName);
        }

        return Result<PlantDto>.Success(plant);
    }
}
