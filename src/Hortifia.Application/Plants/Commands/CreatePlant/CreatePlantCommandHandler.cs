using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Application.Plants.Static;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.CreatePlant;

public class CreatePlantCommandHandler(IPlantsRepository plantsRepository,
    IRoomsRepository roomsRepository,
    ILogger<CreatePlantCommandHandler> logger,
    IUserContext userContext,
    IMediator mediator) : IRequestHandler<CreatePlantCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        //TODO: Add picture upload handling

        var currentUser = userContext.GetCurrentUser();
        var room = await roomsRepository.GetByIdAsync(request.RoomId);

        if (room is null || room.OwnerId != currentUser.Id)
        {
            logger.LogWarning("Room with ID {RoomId} not found or does not belong to the current user.", request.RoomId);
            return Result<int>.Failure("Room not found");
        }

        var plant = Plant.Create(
            name: request.Name,
            commonName: request.CommonName,
            picture: request.Picture?.FileName,
            isNearHeater: request.IsNearHeater,
            lightCondition: request.LightCondition,
            lastWateringDate: request.LastWateringDate,
            roomId: request.RoomId,
            ownerId: currentUser.Id,
            plantApiId: request.PlantApiId,
            room: room);

        var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = request.PlantApiId }, cancellationToken);

        if (!apiPlantResult.IsSuccess)
        {
            logger.LogError("Failed to retrieve plant API data for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, apiPlantResult.ErrorMessage);
            return Result<int>.Failure("Failed to retrieve plant data from external API.");
        }

        var apiPlant = apiPlantResult.Value;

        if (apiPlant is null)
        {
            logger.LogError("No plant data found for PlantApiId: {PlantApiId}.", request.PlantApiId);
            return Result<int>.Failure("No plant data found from external API.");
        }

        var waterRequirementEntry = apiPlant.Data?.FirstOrDefault
            (d => d.Key?.Equals(PlantApiDataKeys.WaterRequirement, StringComparison.OrdinalIgnoreCase) == true);

        var lightRequirementEntry = apiPlant.Data?.FirstOrDefault
            (d => d.Key?.Equals(PlantApiDataKeys.LightRequirement, StringComparison.OrdinalIgnoreCase) == true);

        if (waterRequirementEntry is null || lightRequirementEntry is null)
        {
            logger.LogWarning("No requirement data not found for PlantApiId: {PlantApiId}.", request.PlantApiId);
            return Result<int>.Failure("Requirement data not found from external API.");
        }

        var waterRequirements = PlantDataParser.ParseWaterRequirements(waterRequirementEntry.Value);

        if (!waterRequirements.IsSuccess || waterRequirements.Value is null)
        {
            logger.LogError("Failed to parse water requirements for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, waterRequirements.ErrorMessage);
            return Result<int>.Failure("Failed to parse water requirements from external API data.");
        }

        var lightRequirements = PlantDataParser.ParseLightCondition(lightRequirementEntry.Value);

        if (!lightRequirements.IsSuccess || lightRequirements.Value is null)
        {
            logger.LogError("Failed to parse light conditions for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, lightRequirements.ErrorMessage);
            return Result<int>.Failure("Failed to parse light conditions from external API data.");
        }

        var result = plant.SetExpectedWateringDate(waterRequirements.Value, lightRequirements.Value, currentUser.PrefferedNotificationTime);

        if (result is null || !result.IsSuccess)
        {
            logger.LogError("Failed to set expected watering date");
            return Result<int>.Failure("Failed to set expected watering date");
        }

        var plantId = await plantsRepository.CreateAsync(plant);

        return Result<int>.Success(plantId);
    }
}
