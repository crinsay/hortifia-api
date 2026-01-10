using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Hortifia.Application.Plants.Commands.CreatePlant;

public class CreatePlantCommand : IRequest<Result<PlantDto>>
{
    public string Name { get; init; } = default!;
    public string CommonName { get; init; } = default!;
    public IFormFile? Img { get; init; }
    public bool IsNearHeater { get; init; }
    public LightCondition LightCondition { get; init; } = LightCondition.Medium;
    public DateTime LastWateringDate { get; init; }
    public int RoomId { get; init; }
    public int PlantApiId { get; init; }
}
