using FluentValidation;

namespace Hortifia.Application.Plants.Queries.GetPlants;

public class GetPlantsQueryValidator : AbstractValidator<GetPlantsQuery>
{
    public GetPlantsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be grater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be grater than 0");
    }
}
