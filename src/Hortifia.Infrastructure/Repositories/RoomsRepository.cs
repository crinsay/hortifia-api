using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Plants.Dtos;
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

    public async Task<Room?> GetByIdAsync(int roomId, bool includePlants = false)
    {
        var mainQuery = dbContext.Rooms.AsQueryable();

        if (includePlants)
        {
            mainQuery = mainQuery.Include(r => r.Plants);
        }

        var room = await mainQuery
            .FirstOrDefaultAsync(r => r.Id == roomId);

        return room;
    }

    public async Task<IEnumerable<RoomListDto>> GetAllDtosByUserIdAsync(string userId, 
        string? searchPhrase, 
        int pageNumber,
        int pageSize,
        bool limitToFour = false)
    {
        var query = dbContext.Rooms
            .Where(r => r.OwnerId == userId);

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            var normalized = searchPhrase.ToLower().Trim();
            query = query.Where(r => r.Name.ToLower().Contains(normalized));
        }

        query = query.OrderBy(r => r.Name);

        if (limitToFour)
        {
            query = query.Take(4);
        }
        else
        {
            query = query.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);
        }

        var rooms = await query
            .Select(r => new RoomListDto
            {
                Id = r.Id,
                Name = r.Name,
                UserId = r.OwnerId,
                PlantImgUrls = r.Plants
                    .Where(p => p.ImgBlobName != null)
                    .Select(p => p.ImgBlobName!)
                    .Take(4)
                    .ToList()
            })
            .ToListAsync();

        return rooms;
    }

    public async Task DeleteAsync(Room room)
    {
        dbContext.Rooms.Remove(room);
        await dbContext.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
