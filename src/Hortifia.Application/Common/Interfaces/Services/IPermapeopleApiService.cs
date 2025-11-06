using Hortifia.Application.Plants.Dtos;

namespace Hortifia.Application.Common.Interfaces.Services;

public interface IPermapeopleApiService
{
    Task<IEnumerable<PlantApiDto>?> GetPlantsAsync(int? lastId = null);
    Task<PlantApiDto?> GetPlantByIdAsync(int id);
    Task<IEnumerable<PlantApiDto>?> SearchPlantsAsync(string query);
}

