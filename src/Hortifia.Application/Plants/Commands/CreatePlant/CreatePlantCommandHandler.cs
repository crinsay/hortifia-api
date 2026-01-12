using Hortifia.Application.Common.Helpers;
using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Application.Plants.Static;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.CreatePlant;

public class CreatePlantCommandHandler(IPlantsRepository plantsRepository,
    IRoomsRepository roomsRepository,
    ILogger<CreatePlantCommandHandler> logger,
    IUserContext userContext,
    IMediator mediator,
    IBlobStorageService blobStorageService,
    IUnitOfWork unitOfWork,
    IQuartzSchedulerService quartzSchedulerService,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService,
    IAuthorizationService authorizationService) : IRequestHandler<CreatePlantCommand, Result<PlantDto>>
{
    public async Task<Result<PlantDto>> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var roomId = request.RoomId;

        var room = await roomsRepository.GetByIdAsync(roomId, includePlants: true);

        if (room is null)
        {
            logger.LogWarning("Room with ID {RoomId} not found.", roomId);
            return Result<PlantDto>.Failure("Room not found");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, room, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Room with id {roomId} does not belong to the current user.", roomId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<PlantDto>.Failure("Room not found.");
        }

        var plant = Plant.Create(
            name: request.Name,
            commonName: request.CommonName,
            isNearHeater: request.IsNearHeater,
            lightCondition: request.LightCondition,
            lastWateringDate: request.LastWateringDate,
            roomId: request.RoomId,
            ownerId: currentUser.Id!,
            plantApiId: request.PlantApiId,
            room: room);

        var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = request.PlantApiId }, cancellationToken);

        if (!apiPlantResult.IsSuccess)
        {
            logger.LogError("Failed to retrieve plant API data for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, apiPlantResult.ErrorMessage);
            return Result<PlantDto>.Failure("Failed to retrieve plant data from external API.");
        }

        var apiPlant = apiPlantResult.Value;

        if (apiPlant is null)
        {
            logger.LogError("No plant data found for PlantApiId: {PlantApiId}.", request.PlantApiId);
            return Result<PlantDto>.Failure("No plant data found from external API.");
        }

        if (apiPlant.WaterRequirement is null || apiPlant.LightRequirement is null)
        {
            logger.LogWarning("No requirement data not found for PlantApiId: {PlantApiId}.", request.PlantApiId);
            return Result<PlantDto>.Failure("Requirement data not found from external API.");
        }

        var waterRequirements = PlantDataParser.ParseWaterRequirements(apiPlant.WaterRequirement);

        if (!waterRequirements.IsSuccess || waterRequirements.Value is null)
        {
            logger.LogError("Failed to parse water requirements for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, waterRequirements.ErrorMessage);
            return Result<PlantDto>.Failure("Failed to parse water requirements from external API data.");
        }

        var lightRequirements = PlantDataParser.ParseLightCondition(apiPlant.LightRequirement);

        if (!lightRequirements.IsSuccess || lightRequirements.Value is null)
        {
            logger.LogError("Failed to parse light conditions for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                request.PlantApiId, lightRequirements.ErrorMessage);
            return Result<PlantDto>.Failure("Failed to parse light conditions from external API data.");
        }

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<PlantDto>.Failure("User coordinates not found - probably current user no longer exists.");
        }

        var weather = await weatherApiService.GetLongTermWeatherAsync(latitude.Value, longitude.Value, 16);

        if (weather is null)
        {
            return Result<PlantDto>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperatures is null)
        {
            return Result<PlantDto>.Failure("Incomplete weather data received from external APIs.");
        }

        var result = plant.SetExpectedWateringDate(waterRequirements.Value, 
            lightRequirements.Value, 
            currentUser.PrefferedNotificationTime,
            [.. weather.Temperatures]);

        if (result is null || !result.IsSuccess)
        {
            logger.LogError("Failed to set expected watering date");
            return Result<PlantDto>.Failure("Failed to set expected watering date");
        }
      
        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            int newPlantId = await plantsRepository.CreateAsync(plant);

            var plantImg = request.Img;
            if (plantImg is not null)
            {
                var plantImgName = plantImg.FileName;

                var blobNameResult = BlobHelper.GetBlobName<Plant>(plant.Id, plantImgName);
                if (!blobNameResult.IsSuccess)
                {
                    logger.LogCritical("[{handler}] Couldn't get blob name. BlobHelper might not be up to date!!!", nameof(CreatePlantCommandHandler));
                    return Result.Failure(blobNameResult.ErrorMessage!);
                }

                plant.ImgBlobName = blobNameResult.Value;
                await unitOfWork.SaveChangesAsync();

                var fileExtension = Path.GetExtension(plantImgName).ToLowerInvariant();
                using var stream = plantImg.OpenReadStream();
                await blobStorageService.UploadBlobAsync(stream, plant.ImgBlobName!, fileExtension);
            }

            return Result.Success();
        });

        await quartzSchedulerService.ScheduleWateringNotificationForUserAsync(plant.OwnerId, plant.ExpectedWateringDate);

        var plantDto = PlantDto.CreateFromEntity(plant, temperature: weather.Temperatures.FirstOrDefault() ?? 20);
        plantDto.PlantApiInfo = apiPlant;
        if (plant.ImgBlobName is not null)
        {
            plantDto.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(plant.ImgBlobName);
        }

        var roomPlantsImgBlobNames = room.Plants
            .Where(r => r.ImgBlobName != null)
            .Select(r => r.ImgBlobName!)
            .Take(4);
        foreach (var imgBlobName in roomPlantsImgBlobNames)
        {
            var imgUrl = await blobStorageService.GetBlobSasUrlAsync(imgBlobName);
            plantDto.Room.PlantImgUrls.Add(imgUrl);
        }

        return Result<PlantDto>.Success(plantDto);
    }
}
