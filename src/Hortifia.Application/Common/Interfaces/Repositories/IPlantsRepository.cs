using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IPlantsRepository
{
    Task<int> CreateAsync(Plant plant);
    Task<PlantDto?> GetDtoByIdAsync(int plantId);
    Task<Plant?> GetByIdAsync(int plantId);
    Task SaveChangesAsync();
}
