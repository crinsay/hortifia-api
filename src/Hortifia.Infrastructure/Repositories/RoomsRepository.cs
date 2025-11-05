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

    public async Task<RoomDto?> GetDtoByIdAsync(int roomId)
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

    public async Task<Room?> GetByIdAsync(int roomId)
    {
        var room = await dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);

        return room;
    }


    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();

    public async Task<IEnumerable<RoomListDto>> GetAllDtosByUserIdAsync(string userId, string? searchPhrase)
    {
        var rooms = await dbContext.Rooms
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Where(r => string.IsNullOrEmpty(searchPhrase) ||
                         r.Name.ToLower().Contains(searchPhrase.ToLower().Trim()))
            .Include(r => r.Plants)
            .Select(r => new RoomListDto
            {
                Id = r.Id,
                Name = r.Name,
                UserId = r.UserId,
                PlantImgUrls = r.Plants
                    .Where(p => p.ImgBlobName != null)              
                    .Select(p => p.ImgBlobName!)
                    .Take(4)
                    .ToList()
            })
            .OrderBy(r => r.Name)
            .ToListAsync();

        return rooms;
    }
}
