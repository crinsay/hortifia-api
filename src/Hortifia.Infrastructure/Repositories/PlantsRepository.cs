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
        var plant = await dbContext.Plants.FirstOrDefaultAsync(p => p.Id == plantId);

        return plant;
    }

    public Task SaveChangesAsync()
    => dbContext.SaveChangesAsync();
}
