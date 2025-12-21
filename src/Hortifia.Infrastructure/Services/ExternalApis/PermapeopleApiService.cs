using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using System.Text.Json;
using System.Net.Http.Json;

namespace Hortifia.Infrastructure.Services.ExternalApis;

internal class PermapeopleApiService(HttpClient httpClient) : IPermapeopleApiService
{
    public async Task<IEnumerable<PlantLookupDto>?> GetPlantsAsync(int? lastItemId)
    {
        var requestUrl = "plants";
        if (lastItemId.HasValue)
        {
            requestUrl += $"?last_id={lastItemId.Value}";
        }

        var response = await httpClient.GetFromJsonAsync<PlantsApiResponseDto>(
            requestUrl,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        return response?.Plants?
            .Select(p => new PlantLookupDto
            {
                PlantApiId = p.Id,
                CommonName = p.CommonName ?? string.Empty,
                ScientificName = p.ScientificName ?? string.Empty
            })
            .ToList()
            ?? [];
    }

    public async Task<PlantApiDto?> GetPlantByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<PlantApiDto>($"plants/{id}");
    }

    public async Task<IEnumerable<PlantLookupDto>?> SearchPlantsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Search query cannot be empty.", nameof(query));
        }

        var requestBody = new { q = query };

        var response = await httpClient.PostAsJsonAsync("search", requestBody);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PlantsApiResponseDto>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result?.Plants?
            .Select(p => new PlantLookupDto
            {
                PlantApiId = p.Id,
                CommonName = p.CommonName ?? string.Empty,
                ScientificName = p.ScientificName ?? string.Empty
            })
            .ToList()
            ?? [];
    }
}
