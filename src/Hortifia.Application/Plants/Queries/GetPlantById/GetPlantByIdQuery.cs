using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Queries.GetPlantById;

public class GetPlantByIdQuery : IRequest<Result<PlantDto>>
{
    public required int PlantId { get; init; }
}
