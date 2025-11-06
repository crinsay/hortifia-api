using Hortifia.Application.Plants.Dtos;
using Microsoft.Extensions.Configuration;

namespace Hortifia.Application.Common.Interfaces.Services;

public interface IPermapeopleApiService
{
    Task<IEnumerable<PlantDto>?> GetPlantsAsync();
    Task<PlantDto?> GetPlantByIdAsync(int id);
}

