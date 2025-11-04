using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hortifia.Infrastructure.Repositories;

internal class RoomsRepository(HortifiaDbContext dbContext) : IRoomsRepository
{
    public async Task<int> CreateAsync(Room room)
    {
        dbContext.Rooms.Add(room);
        await dbContext.SaveChangesAsync();

        return room.Id;
    }

    public async Task<RoomDto?> GetByIdAsync(int roomId)
    {
        var room = await dbContext.Rooms
            .Include(r => r.Plants)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Type = r.Type,
                Humidity = r.Humidity,
                Temperature = r.Temperature,
                UserId = r.UserId,
                Plants = r.Plants.ToList()
            })
            .FirstOrDefaultAsync(r => r.Id == roomId);

        return room;
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
