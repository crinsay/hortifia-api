using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Hortifia.Infrastructure.Services;

internal class PermapeopleApiService : IPermapeopleApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration configuration;

    public PermapeopleApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task<IEnumerable<PlantDto>?> GetPlantsAsync(int? lastId = null)
    {
        var endpoint = "plants";
        if (lastId.HasValue)
            endpoint += $"?last_id={lastId.Value}";

        var response = await _httpClient.GetFromJsonAsync<PlantsResponseDto>(
            endpoint,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        return response?.Plants;
    }

    public async Task<PlantDto?> GetPlantByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<PlantDto>($"plants/{id}");
    }

    public async Task<IEnumerable<PlantDto>?> SearchPlantsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query cannot be empty.", nameof(query));

        var requestBody = new { q = query };

        var response = await _httpClient.PostAsJsonAsync("search", requestBody);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PlantsResponseDto>(
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result?.Plants;
    }
}
