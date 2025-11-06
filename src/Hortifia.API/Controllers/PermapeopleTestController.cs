using Hortifia.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermapeopleTestController : ControllerBase
{
    private readonly IPermapeopleApiService _permapeopleApiService;

    public PermapeopleTestController(IPermapeopleApiService permapeopleApiService) => _permapeopleApiService = permapeopleApiService;

    [HttpGet]
    public async Task<IActionResult> GetPlants([FromQuery] int? lastId = null)
    {
        var plants = await _permapeopleApiService.GetPlantsAsync(lastId);
        return Ok(plants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlant(int id)
    {
        var plant = await _permapeopleApiService.GetPlantByIdAsync(id);
        if (plant is null)
            return NotFound();

        return Ok(plant);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchPlants([FromBody] string query)
    {
        var results = await _permapeopleApiService.SearchPlantsAsync(query);
        return Ok(results);
    }

}
