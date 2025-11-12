using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Plants.Commands.WaterPlant;

public class WaterPlantCommand : IRequest<Result>
{
    public int Id { get; init; }
}
