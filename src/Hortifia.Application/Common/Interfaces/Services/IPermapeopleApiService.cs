using Hortifia.Application.Plants.Dtos;

namespace Hortifia.Application.Common.Interfaces.Services;

public interface IPermapeopleApiService
{
    Task<IEnumerable<PlantLookupDto>?> GetPlantsAsync(int? lastItemId);
    Task<PlantApiDto?> GetPlantByIdAsync(int id);
    Task<IEnumerable<PlantLookupDto>?> SearchPlantsAsync(string query);
}
