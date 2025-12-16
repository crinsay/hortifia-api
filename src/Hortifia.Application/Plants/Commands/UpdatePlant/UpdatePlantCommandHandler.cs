using Hortifia.Application.Common.Helpers;
using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Application.Plants.Static;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.UpdatePlant;

public class UpdatePlantCommandHandler(IPlantsRepository plantsRepository,
        IRoomsRepository roomsRepository,
        ILogger<UpdatePlantCommandHandler> logger,
        IMediator mediator,
        IUserContext userContext,
        IBlobStorageService blobStorageService,
        IUnitOfWork unitOfWork,
        IQuartzSchedulerService quartzSchedulerService,
        IIdentityRepository identityRepository,
        IWeatherApiService weatherApiService,
        IAuthorizationService authorizationService) : IRequestHandler<UpdatePlantCommand, Result>
{
    public async Task<Result> Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var roomId = request.RoomId;

        var room = await roomsRepository.GetByIdAsync(roomId);

        if (room is null)
        {
            logger.LogWarning("Room with ID {RoomId} not found or does not belong to the current user.", roomId);
            return Result.Failure("Specified room does not exist.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, room, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Room with id {roomId} does not belong to the current user.", roomId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result.Failure("Specified room does not exist.");
        }

        var plantId = request.PlantId;
        var plantToUpdate = await plantsRepository.GetByIdAsync(plantId);

        if (plantToUpdate is null)
        {
            logger.LogWarning("Plant with ID {PlantId} not found.", currentUser.Id);
            return Result.Failure("Specified plant does not exist.");
        }

        authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, plantToUpdate, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Plant with id {roomId} does not belong to the current user.", plantId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result.Failure("Specified plant does not exist.");
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
            isNearHeater: request.IsNearHeater,
            lightCondition: request.LightCondition,
            lastWateringDate: request.LastWateringDate,
            roomId: request.RoomId,
            plantApiId: request.PlantApiId,
            room: room);

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

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<int>.Failure("User coordinates not found - probably current user no longer exists.");
        }

        var weather = await weatherApiService.GetLongTermWeatherAsync(latitude.Value, longitude.Value, 16);

        if (weather is null)
        {
            return Result<int>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperatures is null)
        {
            return Result<int>.Failure("Incomplete weather data received from external APIs.");
        }

        var result = plantToUpdate.SetExpectedWateringDate(waterRequirements.Value,
            lightRequirements.Value,
            currentUser.PrefferedNotificationTime,
            [.. weather.Temperatures]);

        if (result is null || !result.IsSuccess)
        {
            logger.LogError("Failed to set expected watering date");
            return Result.Failure("Failed to set expected watering date");
        }

        var oldPlantImgBlobName = plantToUpdate.ImgBlobName;
        var newPlantImg = request.Img;
        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            if (newPlantImg is null)
            {
                plantToUpdate.ImgBlobName = null;
                await unitOfWork.SaveChangesAsync();

                if (oldPlantImgBlobName is not null)
                {
                    await blobStorageService.DeleteBlobAsync(oldPlantImgBlobName);
                }
            }
            else
            {
                var newPlantImgName = newPlantImg.FileName;

                var blobNameResult = BlobHelper.GetBlobName<Plant>(plantToUpdate.Id, newPlantImgName);
                if (!blobNameResult.IsSuccess)
                {
                    logger.LogCritical("[{handler}] Couldn't get blob name. BlobHelper might not be up to date!!!", nameof(UpdatePlantCommandHandler));
                    return Result.Failure(blobNameResult.ErrorMessage!);
                }

                plantToUpdate.ImgBlobName = blobNameResult.Value;
                await unitOfWork.SaveChangesAsync();

                var fileExtension = Path.GetExtension(newPlantImgName).ToLowerInvariant();
                using var stream = newPlantImg.OpenReadStream();
                if (oldPlantImgBlobName is null)
                {
                    await blobStorageService.UploadBlobAsync(stream, plantToUpdate.ImgBlobName!, fileExtension);
                }
                else
                {
                    await blobStorageService.ReplaceBlobAsync(
                        newBlobContent: stream,
                        newBlobName: plantToUpdate.ImgBlobName!,
                        newBlobContentType: fileExtension,
                        oldBlobName: oldPlantImgBlobName);
                }
            }

            return Result.Success();
        });

        await plantsRepository.SaveChangesAsync();
        await quartzSchedulerService.ScheduleWateringNotificationForUserAsync(plantToUpdate.OwnerId, plantToUpdate.ExpectedWateringDate);

        return Result.Success();
    }
}
