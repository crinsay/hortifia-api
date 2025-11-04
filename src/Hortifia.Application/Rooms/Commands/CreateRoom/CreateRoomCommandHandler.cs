using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommandHandler(IRoomsRepository roomsRepository,
    ILogger<CreateRoomCommandHandler> logger,
    IUserContext userContext) : IRequestHandler<CreateRoomCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser.Id is null)
        {
            logger.LogWarning("Unauthorized attempt to create a room.");
            return Result<int>.Failure("User is not authenticated.");
        }
                
        var room = Room.Create(request.Name, request.Type, request.Humidity,
            request.Temperature, currentUser.Id);

        var roomId = await roomsRepository.CreateAsync(room);

        return Result<int>.Success(roomId);
    }
}
