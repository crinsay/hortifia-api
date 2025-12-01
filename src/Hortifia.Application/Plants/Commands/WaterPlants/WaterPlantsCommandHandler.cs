using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Application.Plants.Static;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.WaterPlants;

internal class WaterPlantsCommandHandler(IPlantsRepository plantsRepository,
    ILogger<WaterPlantsCommandHandler> logger,
    IMediator mediator,
    IUserContext userContext,
    IQuartzSchedulerService quartzSchedulerService,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService) : IRequestHandler<WaterPlantsCommand, Result>
{
    public async Task<Result> Handle(WaterPlantsCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var plants = await plantsRepository.GetPlantsByIdsAsync(currentUser!.Id!, request.PlantIds);

        if (!plants.Any()) 
        {
            logger.LogWarning("No plants found to water for user {UserId}.", currentUser.Id);
            return Result.Failure("No plants found.");
        }

        foreach (var plant in plants)
        {
            plant.UpdateLastWateringDate();

            var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = plant.PlantApiId }, cancellationToken);

            if (!apiPlantResult.IsSuccess || apiPlantResult.Value is null)
            {
                logger.LogError("Failed to retrieve plant API data for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                    plant.PlantApiId, apiPlantResult.ErrorMessage);
                return Result.Failure("Failed to retrieve plant data from external API.");
            }

            var apiPlant = apiPlantResult.Value;

            var waterRequirementEntry = apiPlant.Data?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.WaterRequirement, StringComparison.OrdinalIgnoreCase) == true);

            var lightRequirementEntry = apiPlant.Data?.FirstOrDefault
                (d => d.Key?.Equals(PlantApiDataKeys.LightRequirement, StringComparison.OrdinalIgnoreCase) == true);

            if (waterRequirementEntry is null || lightRequirementEntry is null)
            {
                logger.LogWarning("No requirement data not found for PlantApiId: {PlantApiId}.", plant.PlantApiId);
                return Result.Failure("Requirement data not found from external API.");
            }

            var waterRequirements = PlantDataParser.ParseWaterRequirements(waterRequirementEntry.Value);

            if (!waterRequirements.IsSuccess || waterRequirements.Value is null)
            {
                logger.LogError("Failed to parse water requirements for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                    plant.PlantApiId, waterRequirements.ErrorMessage);
                return Result.Failure("Failed to parse water requirements from external API data.");
            }

            var lightRequirements = PlantDataParser.ParseLightCondition(lightRequirementEntry.Value);

            if (!lightRequirements.IsSuccess || lightRequirements.Value is null)
            {
                logger.LogError("Failed to parse light conditions for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                    plant.PlantApiId, lightRequirements.ErrorMessage);
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

            var result = plant.SetExpectedWateringDate(waterRequirements.Value,
                lightRequirements.Value,
                currentUser.PrefferedNotificationTime,
                [.. weather.Temperatures]);

            if (result is null || !result.IsSuccess)
            {
                logger.LogError("Failed to set expected watering date");
                return Result.Failure("Failed to set expected watering date");
            }

            await quartzSchedulerService.ScheduleWateringNotificationForUserAsync(plant.OwnerId, plant.ExpectedWateringDate);
        }
        
        await plantsRepository.SaveChangesAsync();

        return Result.Success();
    }
}


