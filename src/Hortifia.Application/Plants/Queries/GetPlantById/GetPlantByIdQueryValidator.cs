using FluentValidation;

namespace Hortifia.Application.Plants.Queries.GetPlantById;

public class GetPlantByIdQueryValidator : AbstractValidator<GetPlantByIdQuery>
{
    public GetPlantByIdQueryValidator()
    {
        RuleFor(x => x.PlantId)
            .GreaterThan(0).WithMessage("Plant Id must be greater than zero.");
    }
}
