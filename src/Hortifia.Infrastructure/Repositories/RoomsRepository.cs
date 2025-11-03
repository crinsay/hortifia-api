using Hortifia.Domain.Common.Interfaces.Repositories;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hortifia.Infrastructure.Repositories;

internal class RoomsRepository(HortifiaDbContext dbContext) : IRoomsRepository
{
    public async Task<Room?> GetByIdAsync(int roomId)
    {
        var room = await dbContext.Rooms
            .Include(r => r.Plants)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        return room;
    }
}
