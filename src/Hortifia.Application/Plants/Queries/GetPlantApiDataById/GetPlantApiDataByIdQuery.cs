using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Queries.GetPlantApiDataById;

public class GetPlantApiDataByIdQuery : IRequest<Result<PlantApiInfoDto>>
{
    public required int PlantApiId { get; init; }
}