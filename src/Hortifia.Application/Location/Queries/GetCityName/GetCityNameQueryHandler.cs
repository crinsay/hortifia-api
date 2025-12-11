using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Location.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Location.Queries.GetCityName;

internal class GetCityNameQueryHandler(ICityApiService cityApiService) : IRequestHandler<GetCityNameQuery, Result<CityNameDto>>
{
    public async Task<Result<CityNameDto>> Handle(GetCityNameQuery request, CancellationToken cancellationToken)
    {
        var cityName = await cityApiService.GetCityNameAsync(request.Latitude, request.Longitude);
        if (string.IsNullOrWhiteSpace(cityName))
        {
            return Result<CityNameDto>.Failure("Couldn't fetch city name.");
        }

        return Result<CityNameDto>.Success(new CityNameDto { CityName = cityName });
    }
}
