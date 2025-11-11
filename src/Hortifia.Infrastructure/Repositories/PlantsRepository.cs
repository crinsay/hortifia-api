using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;

namespace Hortifia.Infrastructure.Repositories;

internal class PlantsRepository(HortifiaDbContext dbContext) : IPlantsRepository
{
    public async Task<int> CreateAsync(Plant plant)
    {
        dbContext.Plants.Add(plant);
        await dbContext.SaveChangesAsync();

        return plant.Id;
    }
}
