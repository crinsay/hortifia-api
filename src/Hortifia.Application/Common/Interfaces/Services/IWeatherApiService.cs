using Hortifia.Application.Weather.Dtos;

namespace Hortifia.Application.Common.Interfaces.Services;

public interface IWeatherApiService
{
    Task<WeatherWithCityDto?> GetCurrentWeatherAsync(double latitude, double longitude);
    Task<DailyWeatherInfoDto?> GetLongTermWeatherAsync(double latitude, double longitude, byte daysSpan = 7);
}
