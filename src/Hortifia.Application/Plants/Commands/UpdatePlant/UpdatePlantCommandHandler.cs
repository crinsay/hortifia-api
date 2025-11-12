using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Application.Plants.Static;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.UpdatePlant;

public class UpdatePlantCommandHandler(IPlantsRepository plantsRepository,
        IRoomsRepository roomsRepository,
        ILogger<UpdatePlantCommandHandler> logger,
        IMediator mediator,
        IUserContext userContext) : IRequestHandler<UpdatePlantCommand, Result>
{
    public async Task<Result> Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.Id))
        {
            logger.LogWarning("Unauthorized attempt to create a plant.");
            return Result.Failure("User is not authenticated.");
        }

        var room = await roomsRepository.GetByIdAsync(request.RoomId);

        if (room is null || room.UserId != currentUser.Id)
        {
            logger.LogWarning("Room with ID {RoomId} not found or does not belong to the current user.", request.RoomId);
            return Result.Failure("Specified room does not exist or does not belong to the user.");
        }

        var plantToUpdate = await plantsRepository.GetByIdAsync(request.Id);

        if (plantToUpdate is null || plantToUpdate.UserId != currentUser.Id)
        {
            logger.LogWarning("Plant with ID {PlantId} not found or does not belong to the current user.", currentUser.Id);
            return Result.Failure("Specified plant does not exist or does not belong to the user.");
        }

        var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = request.PlantApiId }, cancellationToken);

        if (!apiPlantResult.IsSuccess || apiPlantResult.Value is null)
        {
            logger.LogError("Failed to retrieve plant API data for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, apiPlantResult.ErrorMessage);
            return Result.Failure("Failed to retrieve plant data from external API.");
        }

        plantToUpdate.Update(
            name: request.Name,
            commonName: request.CommonName,
            picture: request.Picture?.FileName,
            isNearHeater: request.IsNearHeater,
            lightCondition: request.LightCondition,
            lastWateringDate: request.LastWateringDate,
            roomId: request.RoomId,
            plantApiId: request.PlantApiId,
            room: room
            );

        var apiPlant = apiPlantResult.Value;

        var waterRequirementEntry = apiPlant.Data?.FirstOrDefault
            (d => d.Key?.Equals(PlantApiDataKeys.WaterRequirement, StringComparison.OrdinalIgnoreCase) == true);

        var lightRequirementEntry = apiPlant.Data?.FirstOrDefault
            (d => d.Key?.Equals(PlantApiDataKeys.LightRequirement, StringComparison.OrdinalIgnoreCase) == true);

        if (waterRequirementEntry is null || lightRequirementEntry is null)
        {
            logger.LogWarning("No requirement data not found for PlantApiId: {PlantApiId}.", request.PlantApiId);
            return Result.Failure("Requirement data not found from external API.");
        }

        var waterRequirements = PlantDataParser.ParseWaterRequirements(waterRequirementEntry.Value);

        if (!waterRequirements.IsSuccess || waterRequirements.Value is null)
        {
            logger.LogError("Failed to parse water requirements for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, waterRequirements.ErrorMessage);
            return Result.Failure("Failed to parse water requirements from external API data.");
        }

        var lightRequirements = PlantDataParser.ParseLightCondition(lightRequirementEntry.Value);

        if (!lightRequirements.IsSuccess || lightRequirements.Value is null)
        {
            logger.LogError("Failed to parse light conditions for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, lightRequirements.ErrorMessage);
            return Result.Failure("Failed to parse light conditions from external API data.");
        }

        var result = plantToUpdate.SetExpectedWateringDate(waterRequirements.Value, lightRequirements.Value, currentUser.PrefferedNotificationTime);

        if (result is null || !result.IsSuccess)
        {
            logger.LogError("Failed to set expected watering date");
            return Result.Failure("Failed to set expected watering date");
        }

        await plantsRepository.SaveChangesAsync();

        return Result.Success();
    }
}
