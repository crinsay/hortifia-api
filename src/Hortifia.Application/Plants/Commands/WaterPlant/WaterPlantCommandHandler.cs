using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Application.Plants.Static;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.WaterPlant;

public class WaterPlantCommandHandler(IPlantsRepository plantsRepository,
    ILogger<WaterPlantCommandHandler> logger,
    IMediator mediator,
    IUserContext userContext,
    IQuartzSchedulerService quartzSchedulerService,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService,
    IAuthorizationService authorizationService) : IRequestHandler<WaterPlantCommand, Result<WateredPlantDto>>
{
    public async Task<Result<WateredPlantDto>> Handle(WaterPlantCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var plantId = request.PlantId;
        var userId = currentUser.Id!;

        var plant = await plantsRepository.GetByIdAsync(plantId, true);

        if (plant is null)
        {
            logger.LogWarning("Plant with ID {PlantId} not found for user {UserId}.", plantId, currentUser.Id);
            return Result<WateredPlantDto>.Failure("Plant not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, plant, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Plant with id {plantId} does not belong to the current user.", plantId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<WateredPlantDto>.Failure("Plant not found.");
        }

        plant.UpdateLastWateringDate();

        var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = plant.PlantApiId }, cancellationToken);

        if (!apiPlantResult.IsSuccess || apiPlantResult.Value is null)
        {
            logger.LogError("Failed to retrieve plant API data for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                plant.PlantApiId, apiPlantResult.ErrorMessage);
            return Result<WateredPlantDto>.Failure("Failed to retrieve plant data from external API.");
        }

        var apiPlant = apiPlantResult.Value;

        if (apiPlant.WaterRequirement is null || apiPlant.LightRequirement is null)
        {
            logger.LogWarning("No requirement data not found for PlantApiId: {PlantApiId}.", plant.PlantApiId);
            return Result<WateredPlantDto>.Failure("Requirement data not found from external API.");
        }

        var waterRequirements = PlantDataParser.ParseWaterRequirements(apiPlant.WaterRequirement);

        if (!waterRequirements.IsSuccess || waterRequirements.Value is null)
        {
            logger.LogError("Failed to parse water requirements for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                plant.PlantApiId, waterRequirements.ErrorMessage);
            return Result<WateredPlantDto>.Failure("Failed to parse water requirements from external API data.");
        }

        var lightRequirements = PlantDataParser.ParseLightCondition(apiPlant.LightRequirement);

        if (!lightRequirements.IsSuccess || lightRequirements.Value is null)
        {
            logger.LogError("Failed to parse light conditions for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                plant.PlantApiId, lightRequirements.ErrorMessage);
            return Result<WateredPlantDto>.Failure("Failed to parse light conditions from external API data.");
        }

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<WateredPlantDto>.Failure("User coordinates not found - probably current user no longer exists.");
        }

        var weather = await weatherApiService.GetLongTermWeatherAsync(latitude.Value, longitude.Value, 16);

        if (weather is null)
        {
            return Result<WateredPlantDto>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperatures is null)
        {
            return Result<WateredPlantDto>.Failure("Incomplete weather data received from external APIs.");
        }

        var result = plant.SetExpectedWateringDate(waterRequirements.Value,
            lightRequirements.Value,
            currentUser.PrefferedNotificationTime,
            [.. weather.Temperatures]);

        if (result is null || !result.IsSuccess)
        {
            logger.LogError("Failed to set expected watering date");
            return Result<WateredPlantDto>.Failure("Failed to set expected watering date");
        }

        await plantsRepository.SaveChangesAsync();
        await quartzSchedulerService.ScheduleWateringNotificationForUserAsync(plant.OwnerId, plant.ExpectedWateringDate);

        var todaysTemperature = weather.Temperatures.FirstOrDefault() ?? 20f;
        var wateredPlantDto = WateredPlantDto.CreateFromEntity(plant);
        wateredPlantDto.IsOverexposedToSunlight = plant.LightCondition == LightCondition.High && todaysTemperature > 30;

        return Result<WateredPlantDto>.Success(wateredPlantDto);
    }
}
