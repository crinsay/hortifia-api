using FluentValidation;

namespace Hortifia.Application.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryValidator : AbstractValidator<GetRoomByIdQuery>
{
    public GetRoomByIdQueryValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("RoomId must be greater than zero.");
    }
}
