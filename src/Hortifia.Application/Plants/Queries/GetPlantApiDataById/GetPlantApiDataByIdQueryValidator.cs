using FluentValidation;

namespace Hortifia.Application.Plants.Queries.GetPlantApiDataById;

public class GetPlantApiDataByIdQueryValidator : AbstractValidator<GetPlantApiDataByIdQuery>
{
    public GetPlantApiDataByIdQueryValidator()
    {
        RuleFor(x => x.PlantApiId)
            .GreaterThan(0).WithMessage("Plant API Id must be greater than zero.");
    }
}
