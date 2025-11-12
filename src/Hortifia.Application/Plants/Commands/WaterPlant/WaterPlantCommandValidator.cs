using FluentValidation;

namespace Hortifia.Application.Plants.Commands.WaterPlant;

public class WaterPlantCommandValidator : AbstractValidator<WaterPlantCommand>
{
    public WaterPlantCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0)
            .WithMessage("Plant ID must be a positive integer.");
    }
}
