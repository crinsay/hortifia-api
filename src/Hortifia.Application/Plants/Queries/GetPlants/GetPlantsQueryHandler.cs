using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Queries.GetPlants;

public class GetPlantsQueryHandler(IPlantsRepository plantsRepository,
    IUserContext userContext,
    IBlobStorageService blobStorageService,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService) : IRequestHandler<GetPlantsQuery, Result<IEnumerable<PlantListDto>>>
{
    public async Task<Result<IEnumerable<PlantListDto>>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<IEnumerable<PlantListDto>>.Failure("User coordinates not found - probably current user no longer exists.");
        }

        var weather = await weatherApiService.GetLongTermWeatherAsync(latitude.Value, longitude.Value, 1);

        if (weather is null)
        {
            return Result<IEnumerable<PlantListDto>>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperatures is null)
        {
            return Result<IEnumerable<PlantListDto>>.Failure("Incomplete weather data received from external APIs.");
        }

        var plants = await plantsRepository.GetDtosByUserIdAsync(currentUser.Id!,
            request.SearchPhrase,
            request.PageNumber,
            request.PageSize,
            request.OnlyFavourites,
            request.LimitToFour,
            request.OnlyPlantsInNeed,
            request.RoomId,
            weather.Temperatures.First() ?? 20);

        foreach (var plant in plants)
        {
            var imgBlobName = plant.ImgUrl;
            if (imgBlobName is not null)
            {
                plant.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(imgBlobName);
            }
        }

        return Result<IEnumerable<PlantListDto>>.Success(plants);
    }
}
