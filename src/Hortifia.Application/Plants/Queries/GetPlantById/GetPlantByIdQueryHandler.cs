using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
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
    IMediator mediator) : IRequestHandler<GetPlantByIdQuery, Result<PlantDto>>
{
    public async Task<Result<PlantDto>> Handle(GetPlantByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.Id))
        {
            logger.LogWarning("Unauthorized attempt to create a room.");
            return Result<PlantDto>.Failure("User is not authenticated.");
        }

        var plant = await plantsRepository.GetDtoByIdAsync(request.PlantId);

        if (plant == null) 
        {
            logger.LogInformation("Plant with ID {PlantId} not found for user {UserId}.", request.PlantId, currentUser.Id);
            return Result<PlantDto>.Failure("Plant not found.");
        }

        if (plant.Room is null || plant.Room.UserId != currentUser.Id)
        {
            logger.LogWarning("User {UserId} attempted to access plant {PlantId} which they do not own.", currentUser.Id, request.PlantId);
            return Result<PlantDto>.Failure("Access denied.");
        }

        var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = plant.PlantApiId }, cancellationToken);

        if (!apiPlantResult.IsSuccess || apiPlantResult.Value == null)
        {
            logger.LogWarning("Failed to retrieve API data for plant {PlantId}.", request.PlantId);
            return Result<PlantDto>.Failure("Failed to retrieve plant API data.");
        }

        var apiPlantData = apiPlantResult.Value.Data;

        // Map apiPlantResult.Value to plant api info DTO as needed
        var plantApiInfoDto = new PlantApiInfoDto
        {
            ScientificName = apiPlantResult.Value.ScientificName,
            Description = apiPlantResult.Value.Description,

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

        return Result<PlantDto>.Success(plant);
    }
}
