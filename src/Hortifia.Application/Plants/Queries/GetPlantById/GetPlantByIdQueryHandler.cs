using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Plants.Queries.GetPlantApiDataById;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Plants.Queries.GetPlantById;

public class GetPlantByIdQueryHandler(IPlantsRepository plantsRepository,
    ILogger<GetPlantByIdQueryHandler> logger,
    IUserContext userContext,
    IMediator mediator,
    IBlobStorageService blobStorageService,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService,
    IAuthorizationService authorizationService) : IRequestHandler<GetPlantByIdQuery, Result<PlantDto>>
{
    public async Task<Result<PlantDto>> Handle(GetPlantByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<PlantDto>.Failure("User coordinates not found - probably current user no longer exists.");
        }

        var weather = await weatherApiService.GetLongTermWeatherAsync(latitude.Value, longitude.Value, 1);

        if (weather is null)
        {
            return Result<PlantDto>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperatures is null)
        {
            return Result<PlantDto>.Failure("Incomplete weather data received from external APIs.");
        }

        var plantId = request.PlantId;
        var plant = await plantsRepository.GetByIdAsync(plantId, includeRoomWithItsPlants: true);

        if (plant is null) 
        {
            logger.LogInformation("User {UserId} attempted to access plant {PlantId} which they do not own.", currentUser.Id, plantId);
            return Result<PlantDto>.Failure("Plant not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, plant, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Plant with id {roomId} does not belong to the current user.", plantId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<PlantDto>.Failure("Plant not found.");
        }

        var apiPlantResult = await mediator.Send(new GetPlantApiDataByIdQuery { PlantApiId = plant.PlantApiId }, cancellationToken);

        if (!apiPlantResult.IsSuccess || apiPlantResult.Value is null)
        {
            logger.LogWarning("Failed to retrieve API data for plant {PlantId}.", request.PlantId);
            return Result<PlantDto>.Failure("Failed to retrieve plant API data.");
        }

        var apiPlantData = apiPlantResult.Value;

        var plantDto = PlantDto.CreateFromEntity(plant, weather.Temperatures.First() ?? 20);

        plantDto.PlantApiInfo = apiPlantData;

        var plantImgBlobName = plant.ImgBlobName;
        if (plantImgBlobName is not null)
        {
            plantDto.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(plantImgBlobName);
        }

        var roomPlantsImgBlobNames = plant.Room.Plants
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
