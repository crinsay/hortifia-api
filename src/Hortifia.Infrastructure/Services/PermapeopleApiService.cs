using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Hortifia.Infrastructure.Services;

internal class PermapeopleApiService(HttpClient httpClient) : IPermapeopleApiService
{
    public async Task<IEnumerable<PlantApiDto>?> GetPlantsAsync(int? lastId = null)
    {
        var endpoint = "plants";
        if (lastId.HasValue)
            endpoint += $"?last_id={lastId.Value}";

        var response = await httpClient.GetFromJsonAsync<PlantsApiResponseDto>(
            endpoint,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        return response?.Plants;
    }

    public async Task<PlantApiDto?> GetPlantByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<PlantApiDto>($"plants/{id}");
    }

    public async Task<IEnumerable<PlantApiDto>?> SearchPlantsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query cannot be empty.", nameof(query));

        var requestBody = new { q = query };

        var response = await httpClient.PostAsJsonAsync("search", requestBody);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PlantsApiResponseDto>(
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result?.Plants;
    }
}
