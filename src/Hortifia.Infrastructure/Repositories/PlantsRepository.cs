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

    public async Task<PlantDto?> GetDtoByIdAsync(int plantId, float temperature = 20)
    {
        var now = DateTime.UtcNow;

        var plant = await dbContext.Plants
            .Select(p => new PlantDto
            {
                Id = p.Id,
                Name = p.Name,
                CommonName = p.CommonName,
                ImgUrl = p.ImgBlobName, // Will be replaced with generated url in app handler.
                IsNearHeater = p.IsNearHeater,
                LightCondition = p.LightCondition,
                LastWateringDate = p.LastWateringDate,
                ExpectedWateringDate = p.ExpectedWateringDate,
                IsFavourite = p.IsFavourite,
                PlantApiId = p.PlantApiId,
                IsInNeed = (Math.Max((int)Math.Floor(
                100 - (now - p.LastWateringDate).TotalDays /
                      (p.ExpectedWateringDate - p.LastWateringDate).TotalDays * 100), 0) < 20) 
                      || (p.LightCondition == LightCondition.High && temperature > 30),
                Room = new RoomListDto
                {
                    Id = p.Room.Id,
                    Name = p.Room.Name,
                    UserId = p.Room.OwnerId,
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

    public async Task<IEnumerable<Plant>> GetPlantsByIdsAsync(string userId, List<int> PlantIds)
    {
        var plants = await dbContext.Plants
            .Where(p => p.OwnerId == userId)
            .Where(p => PlantIds.Contains(p.Id))
            .Include(p => p.Room)
            .ToListAsync();

        return plants;
    }

    public async Task<IEnumerable<PlantListDto>> GetDtosByUserIdAsync(string userId, 
        string? searchPhrase,
        int pageNumber, 
        int pageSize, 
        bool onlyFavourites, 
        bool limitToFour, 
        bool onlyPlantsInNeed,
        float temperature = 20)
    {
        var now = DateTime.UtcNow;

        var query = dbContext.Plants
            .Where(p => p.OwnerId == userId);

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            var searchPhraseLower = searchPhrase.ToLower().Trim();

            query = query.Where(p => p.Name.ToLower().Contains(searchPhraseLower) 
                || p.CommonName.ToLower().Contains(searchPhraseLower));
        }

        if (onlyFavourites)
        {
            query = query.Where(p => p.IsFavourite);
        }

        query = query.OrderBy(p => p.Name);

        if (limitToFour)
        {
            query = query.Take(4);
        }
        else
        {
            query = query.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);
        }

        var plants = await query
            .Select(p => new PlantListDto
            {
                Id = p.Id,
                Name = p.Name,
                CommonName = p.CommonName,
                ImgUrl = p.ImgBlobName, // Will be replaced with generated url in app handler.
                LightCondition = p.LightCondition,
                ExpectedWateringDate = p.ExpectedWateringDate,
                IsFavourite = p.IsFavourite,
                PlantApiId = p.PlantApiId,
                RoomId = p.RoomId,
                WateringStatus = Math.Max((int)Math.Floor(
                    100 - (now - p.LastWateringDate).TotalDays /
                    (p.ExpectedWateringDate - p.LastWateringDate).TotalDays * 100), 0),
                DaysToNextWatering = (int)Math.Ceiling(Math.Max((p.ExpectedWateringDate - now).TotalDays, 0)),
                IsInNeed = (Math.Max((int)Math.Floor(
                100 - (now - p.LastWateringDate).TotalDays /
                      (p.ExpectedWateringDate - p.LastWateringDate).TotalDays * 100), 0) < 20) 
                      || (p.LightCondition == LightCondition.High && temperature > 30)
            }).ToListAsync();

        if (onlyPlantsInNeed)
        {
            plants = [.. plants.Where(p => p.IsInNeed)];
        }

        return plants;
    }

    public async Task<IEnumerable<PlantNameDto>> GetPlantsToNotificationAsync(string userId)
    {
        var plants = await dbContext.Plants
            .Where(p => p.OwnerId == userId)
            .Where(p => p.ExpectedWateringDate.Date <= DateTime.UtcNow)
            .Select(p => new PlantNameDto
            {
                Name = p.Name
            })
            .ToListAsync();

        return plants;
    }

    public async Task<IEnumerable<string>> GetBlobNamesByUserIdAsync(string userId)
    {
        var blobNames = await dbContext.Plants
            .Where(p => p.OwnerId == userId
                   && p.ImgBlobName != null)
            .Select(p => p.ImgBlobName!)
            .ToListAsync();

        return blobNames;
    }

    public async Task DeleteAsync(Plant plant)
    {
        dbContext.Plants.Remove(plant);
        await dbContext.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
