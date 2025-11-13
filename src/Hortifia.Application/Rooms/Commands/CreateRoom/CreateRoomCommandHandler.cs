using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommandHandler(IRoomsRepository roomsRepository,
    IUserContext userContext) : IRequestHandler<CreateRoomCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var room = Room.Create(
            name: request.Name, 
            type: request.Type, 
            humidity: request.Humidity,
            temperature: request.Temperature, 
            userId: currentUser.Id!);

        var roomId = await roomsRepository.CreateAsync(room);

        return Result<int>.Success(roomId);
    }
}
