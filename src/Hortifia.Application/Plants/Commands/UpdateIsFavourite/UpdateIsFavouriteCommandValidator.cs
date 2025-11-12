using FluentValidation;

namespace Hortifia.Application.Plants.Commands.UpdateIsFavourite;

public class UpdateIsFavouriteCommandValidator : AbstractValidator<UpdateIsFavouriteCommand>
{
    public UpdateIsFavouriteCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0)
            .WithMessage("Plant ID must be a positive integer.");
    }
}
