using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Weather.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Weather.Queries.GetCurrentWeather;

public class GetCurrentWeatherQueryHandler(IIdentityRepository identityRepository,
    IUserContext userContext,
    IWeatherApiService weatherApiService) : IRequestHandler<GetCurrentWeatherQuery, Result<WeatherWithCityDto>>
{
    public async Task<Result<WeatherWithCityDto>> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<WeatherWithCityDto>.Failure("User coordinates not found - current user probably no longer exists.");
        }

        var weather = await weatherApiService.GetCurrentWeatherAsync(latitude.Value, longitude.Value);

        if (weather is null) 
        {
            return Result<WeatherWithCityDto>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperature is null || weather.Code is null)
        {
            return Result<WeatherWithCityDto>.Failure("Incomplete weather data received from external weather API.");
        }

        return Result<WeatherWithCityDto>.Success(weather);
    }
}
