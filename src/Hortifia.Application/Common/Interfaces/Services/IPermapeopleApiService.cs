using Hortifia.Application.Plants.Dtos;

namespace Hortifia.Application.Common.Interfaces.Services;

public interface IPermapeopleApiService
{
    Task<IEnumerable<PlantDto>?> GetPlantsAsync(int? lastId = null);
    Task<PlantDto?> GetPlantByIdAsync(int id);
    Task<IEnumerable<PlantDto>?> SearchPlantsAsync(string query);
}

