using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Hortifia.Infrastructure.Repositories;

internal class PlantsRepository(HortifiaDbContext dbContext) : IPlantsRepository
{
    public async Task<int> CreateAsync(Plant plant)
    {
        dbContext.Plants.Add(plant);
        await dbContext.SaveChangesAsync();

        return plant.Id;
    }

    public async Task<PlantDto?> GetDtoByIdAsync(int plantId)
    {
        var plant = await dbContext.Plants
            .Select(p => new PlantDto
            {
                Id = p.Id,
                Name = p.Name,
                CommonName = p.CommonName,
                ImageBlobName = p.ImgBlobName,
                IsNearHeater = p.IsNearHeater,
                LightCondition = p.LightCondition,
                LastWateringDate = p.LastWateringDate,
                ExpectedWateringDate = p.ExpectedWateringDate,
                IsFavourite = p.IsFavourite,
                PlantApiId = p.PlantApiId,

                Room = new RoomListDto
                {
                    Id = p.Room.Id,
                    Name = p.Room.Name,
                    UserId = p.Room.UserId,
                    PlantImgUrls = p.Room.Plants
                        .Where(p => p.ImgBlobName != null)
                        .Select(p => p.ImgBlobName!)
                        .Take(4)
                        .ToList()
                }
            })
            .FirstOrDefaultAsync(p => p.Id == plantId);

        return plant;
    }

    public async Task<Plant?> GetByIdAsync(int plantId)
    {
        var plant = await dbContext.Plants
            .Include(p => p.Room)
            .FirstOrDefaultAsync(p => p.Id == plantId);

        return plant;
    }

    public async Task<IEnumerable<PlantListDto>> GetAllDtosByUserIdAsync(string userId, string? searchPhrase, int pageNumber, int pageSize)
    {
        var now = DateTime.UtcNow;

        var plants = await dbContext.Plants
            .Where(p => p.UserId == userId)
            .Where(p => string.IsNullOrEmpty(searchPhrase) ||
                         p.Name.ToLower().Contains(searchPhrase.ToLower().Trim()) ||
                         p.CommonName.ToLower().Contains(searchPhrase.ToLower().Trim()))
            .Select(p => new PlantListDto
            {
                Id = p.Id,
                Name = p.Name,
                CommonName = p.CommonName,
                ImageBlobName = p.ImgBlobName,
                ExpectedWateringDate = p.ExpectedWateringDate,
                IsFavourite = p.IsFavourite,
                PlantApiId = p.PlantApiId,
                RoomId = p.RoomId,
                WateringStatus = (int)Math.Floor(100 - (now - p.LastWateringDate).TotalDays
                         / (p.ExpectedWateringDate - p.LastWateringDate).TotalDays * 100)
            })
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize) 
            .Take(pageSize)
            .ToListAsync();

        return plants;
    }

    public async Task DeleteAsync(Plant plant)
    {
        dbContext.Plants.Remove(plant);
        await dbContext.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    => dbContext.SaveChangesAsync();
}
