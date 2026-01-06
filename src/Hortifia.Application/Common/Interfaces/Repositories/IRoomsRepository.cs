using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IRoomsRepository
{
    Task<Room?> GetByIdAsync(int roomId);
    Task<IEnumerable<RoomListDto>> GetAllDtosByUserIdAsync(string userId, 
        string? searchPhrase,
        int pageNumber,
        int pageSize,
        bool limitToFour = false);
    Task<int> CreateAsync(Room room);
    Task DeleteAsync(Room room);
    Task SaveChangesAsync();
}
