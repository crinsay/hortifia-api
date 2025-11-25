using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Rooms.Queries.GetRooms;

public class GetRoomsQueryHandler(IRoomsRepository roomsRepository,
    IUserContext userContext,
    IBlobStorageService blobStorageService) : IRequestHandler<GetRoomsQuery, Result<IEnumerable<RoomListDto>>>
{
    public async Task<Result<IEnumerable<RoomListDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var rooms = await roomsRepository.GetAllDtosByUserIdAsync(currentUser.Id!, 
            request.SearchPhrase, 
            request.PageNumber, 
            request.PageSize, 
            request.LimitToFour);

        foreach (var room in rooms)
        {
            var plantsImgBlobName = room.PlantImgUrls.ToList();
            room.PlantImgUrls.Clear();
            foreach (var plantImgBlobName in plantsImgBlobName)
            {
                var plantImgUrl = await blobStorageService.GetBlobSasUrlAsync(plantImgBlobName);
                room.PlantImgUrls.Add(plantImgUrl);
            }
        }

        return Result<IEnumerable<RoomListDto>>.Success(rooms);
    }
}
