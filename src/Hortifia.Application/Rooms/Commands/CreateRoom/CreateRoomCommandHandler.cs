using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;

namespace Hortifia.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommandHandler(IRoomsRepository roomsRepository,
    IUserContext userContext) : IRequestHandler<CreateRoomCommand, Result<RoomDto>>
{
    public async Task<Result<RoomDto>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var room = Room.Create(
            name: request.Name, 
            type: request.Type, 
            humidity: request.Humidity,
            temperature: request.Temperature, 
            userId: currentUser.Id!);

        var _ = await roomsRepository.CreateAsync(room);

        return Result<RoomDto>.Success(RoomDto.CreateFromEntity(room));
    }
}
