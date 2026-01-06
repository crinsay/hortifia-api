using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Plants.Dtos;
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

    public async Task<Plant?> GetByIdAsync(int plantId, bool includeRoomWithItsPlants = false)
    {
        var mainQuery = dbContext.Plants.AsQueryable();

        if (includeRoomWithItsPlants)
        {
            mainQuery = mainQuery.Include(p => p.Room)
                .ThenInclude(r => r.Plants);
        }

        var plant = await mainQuery
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
        int? roomId = null,
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

        if (onlyPlantsInNeed)
        {
            query = query.Where(p =>
                100 -
                (EF.Functions.DateDiffMinute(p.LastWateringDate, now) * 100.0 /
                 EF.Functions.DateDiffMinute(p.LastWateringDate, p.ExpectedWateringDate)) < 20
                ||
                (p.LightCondition == LightCondition.High && temperature > 30)
            );
        }

        if (roomId is not null)
        {
            query = query.Where(p => p.RoomId == roomId);
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
                IsFavourite = p.IsFavourite,
                WateringStatus = Math.Max((int)Math.Floor(
                    100 - (now - p.LastWateringDate).TotalDays /
                    (p.ExpectedWateringDate - p.LastWateringDate).TotalDays * 100), 0),
                DaysToNextWatering = (int)Math.Ceiling(Math.Max((p.ExpectedWateringDate - now).TotalDays, 0)),
                IsWateringNeeded = (Math.Max((int)Math.Floor(
                    100 - (now - p.LastWateringDate).TotalDays /
                    (p.ExpectedWateringDate - p.LastWateringDate).TotalDays * 100), 0) < 20),
                IsOverexposedToSunlight = (p.LightCondition == LightCondition.High && temperature > 30)
            })
            .ToListAsync();

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
