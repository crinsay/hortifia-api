using Hortifia.Domain.Entities;
using Hortifia.Application.Rooms.Dtos;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IRoomsRepository
{
    Task<RoomDto?> GetByIdAsync(int roomId);
    Task<int> CreateAsync(Room room);
    Task SaveChangesAsync();
}
