using FluentValidation;

namespace Hortifia.Application.Plants.Commands.WaterPlants;

public class WaterPlantsCommandValidator : AbstractValidator<WaterPlantsCommand>
{
    public WaterPlantsCommandValidator()
    {
        RuleForEach(p => p.PlantIds)
            .GreaterThan(0)
            .WithMessage("Each PlantId must be greater than 0.");
    }
}
