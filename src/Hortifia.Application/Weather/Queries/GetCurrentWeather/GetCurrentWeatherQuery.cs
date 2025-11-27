using Hortifia.Application.Weather.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Weather.Queries.GetCurrentWeather;

public class GetCurrentWeatherQuery : IRequest<Result<WeatherWithCityDto>>
{
}
