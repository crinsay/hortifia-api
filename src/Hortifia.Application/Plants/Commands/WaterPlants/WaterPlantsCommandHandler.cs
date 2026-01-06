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
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Commands.WaterPlants;

internal class WaterPlantsCommandHandler(IPlantsRepository plantsRepository,
    ILogger<WaterPlantsCommandHandler> logger,
    IMediator mediator,
    IUserContext userContext,
    IQuartzSchedulerService quartzSchedulerService,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService) : IRequestHandler<WaterPlantsCommand, Result<IEnumerable<WateredPlantDto>>>
{
    public async Task<Result<IEnumerable<WateredPlantDto>>> Handle(WaterPlantsCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var plants = await plantsRepository.GetPlantsByIdsAsync(currentUser!.Id!, request.PlantIds);

        if (!plants.Any()) 
        {
            logger.LogWarning("No plants found to water for user {UserId}.", currentUser.Id);
            return Result<IEnumerable<WateredPlantDto>>.Failure("No plants found.");
        }

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<IEnumerable<WateredPlantDto>>.Failure("User coordinates not found - probably current user no longer exists.");
        }

        var weather = await weatherApiService.GetLongTermWeatherAsync(latitude.Value, longitude.Value, 16);

        if (weather is null)
        {
            return Result<IEnumerable<WateredPlantDto>>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperatures is null)
        {
            return Result<IEnumerable<WateredPlantDto>>.Failure("Incomplete weather data received from external APIs.");
        }

        var results = new List<Result>();

        await Parallel.ForEachAsync(plants,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            },
            async (plant, _) =>
            {
                plant.UpdateLastWateringDate();

                var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = plant.PlantApiId }, cancellationToken);

                if (!apiPlantResult.IsSuccess || apiPlantResult.Value is null)
                {
                    logger.LogError("Failed to retrieve plant API data for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                        plant.PlantApiId, apiPlantResult.ErrorMessage);
                    results.Add(Result.Failure("Failed to retrieve plant data from external API."));
                    return;
                }

                var apiPlant = apiPlantResult.Value;

                if (apiPlant.WaterRequirement is null || apiPlant.LightRequirement is null)
                {
                    logger.LogWarning("No requirement data not found for PlantApiId: {PlantApiId}.", plant.PlantApiId);
                    results.Add(Result.Failure("Requirement data not found from external API."));
                    return;
                }

                var waterRequirements = PlantDataParser.ParseWaterRequirements(apiPlant.WaterRequirement);

                if (!waterRequirements.IsSuccess || waterRequirements.Value is null)
                {
                    logger.LogError("Failed to parse water requirements for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                        plant.PlantApiId, waterRequirements.ErrorMessage);
                    results.Add(Result.Failure("Failed to parse water requirements from external API data."));
                    return;
                }

                var lightRequirements = PlantDataParser.ParseLightCondition(apiPlant.LightRequirement);

                if (!lightRequirements.IsSuccess || lightRequirements.Value is null)
                {
                    logger.LogError("Failed to parse light conditions for PlantApiId: {PlantApiId}. Error: {ErrorMessage}",
                        plant.PlantApiId, lightRequirements.ErrorMessage);
                    results.Add(Result.Failure("Failed to parse light conditions from external API data."));
                    return;
                }

                var result = plant.SetExpectedWateringDate(waterRequirements.Value,
                    lightRequirements.Value,
                    currentUser.PrefferedNotificationTime,
                    [.. weather.Temperatures]);

                if (result is null || !result.IsSuccess)
                {
                    logger.LogError("Failed to set expected watering date");
                    results.Add(Result.Failure("Failed to set expected watering date"));
                    return;
                }

                results.Add(Result.Success());
            });

        if (results.Any(r => !r.IsSuccess))
        {
            return Result<IEnumerable<WateredPlantDto>>.Failure("One or more plants failed to be watered.");
        }

        foreach (var plant in plants)
        {
            await quartzSchedulerService.ScheduleWateringNotificationForUserAsync(plant.OwnerId, plant.ExpectedWateringDate);
        }
        
        await plantsRepository.SaveChangesAsync();

        var todaysTemperature = weather.Temperatures.FirstOrDefault() ?? 20f;
        var wateredPlantDtos = new List<WateredPlantDto>();
        foreach(var plant in plants)
        {
            var wateredPlantDto = WateredPlantDto.CreateFromEntity(plant);
            wateredPlantDto.IsOverexposedToSunlight = plant.LightCondition == LightCondition.High && todaysTemperature > 30;
            wateredPlantDtos.Add(wateredPlantDto);         
        }

        return Result<IEnumerable<WateredPlantDto>>.Success(wateredPlantDtos);
    }
}


