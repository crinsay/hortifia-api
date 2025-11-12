using FluentValidation;
using Hortifia.Application.Extensions;
using Hortifia.Application.Plants.Commands.CreatePlant;

namespace Hortifia.Application.Plants.Commands.UpdatePlant;

public class UpdatePlantCommandValidator : AbstractValidator<UpdatePlantCommand>
{
    public UpdatePlantCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Plant name cannot exceed 20 characters.");

        RuleFor(p => p.CommonName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Common name cannot exceed 100 characters.");

        RuleFor(p => p.RoomId)
            .GreaterThan(0)
            .WithMessage("Room ID must be a positive integer.");

        RuleFor(p => p.Picture)
            .Custom((value, context) =>
            {
                if (value is not null)
                {
                    if (!value.IsImage())
                    {
                        context.AddFailure("Img", "Not allowed file extension.");
                    }

                    if (value.FileName.Length > 1024)
                    {
                        context.AddFailure("Img", "Img file name is too long.");
                    }
                }
            });

        RuleFor(p => p.PlantApiId)
            .GreaterThan(0)
            .WithMessage("API Plant ID must be a positive integer.");
    }
}
