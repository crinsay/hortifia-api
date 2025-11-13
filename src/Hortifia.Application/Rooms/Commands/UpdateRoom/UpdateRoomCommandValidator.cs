using FluentValidation;

namespace Hortifia.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {   
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Room name cannot exceed 20 characters.");

        RuleFor(r => r.Type)
            .IsInEnum()
            .WithMessage("Invalid room type.");

        RuleFor(r => r.Humidity)
            .InclusiveBetween((byte)0, (byte)100)
            .WithMessage("Humidity must be between 0 and 100%.");

        RuleFor(r => r.Temperature)
            .InclusiveBetween(-30f, 50f)
            .Must(t => Math.Abs(t * 10 - Math.Round(t * 10)) < 0.0001)
            .WithMessage("Temperature must be between -30°C and 50°C and have at most one decimal place.");
    }
}
