using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Entities;
using MediatR;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IRoomsRepository
{
    Task<RoomDto?> GetDtoByIdAsync(int roomId);
    Task<Room?> GetByIdAsync(int roomId);
    Task<IEnumerable<RoomListDto>> GetAllDtosByUserIdAsync(string userId, string? searchPhrase);
    Task<int> CreateAsync(Room room);
    Task DeleteAsync(Room room);
    Task SaveChangesAsync();
}
