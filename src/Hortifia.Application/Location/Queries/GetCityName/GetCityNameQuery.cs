using Hortifia.Application.Location.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Location.Queries.GetCityName;

public class GetCityNameQuery : IRequest<Result<CityNameDto>>
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
