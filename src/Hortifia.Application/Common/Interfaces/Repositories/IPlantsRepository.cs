using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IPlantsRepository
{
    Task<int> CreateAsync(Plant plant);
    Task<PlantDto?> GetDtoByIdAsync(int plantId);
    Task<Plant?> GetByIdAsync(int plantId);
    Task SaveChangesAsync();
    Task DeleteAsync(Plant plant);
    Task<IEnumerable<PlantListDto>> GetDtosByUserIdAsync(string userId,
        string? searchPhrase,
        int pageNumber,
        int pageSize,
        bool onlyFavourites = false,
        bool limitToFour = false,
        bool onlyPlantsInNeed = false);
    Task<IEnumerable<Plant>> GetPlantsByIdsAsync(string userId, List<int> PlantIds);
}
