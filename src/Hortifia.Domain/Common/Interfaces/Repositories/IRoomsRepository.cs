using Hortifia.Domain.Entities;

namespace Hortifia.Domain.Common.Interfaces.Repositories;

public interface IRoomsRepository
{
    Task<Room?> GetByIdAsync(int roomId);
}
