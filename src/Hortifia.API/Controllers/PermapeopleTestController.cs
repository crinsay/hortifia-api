using Hortifia.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermapeopleTestController : ControllerBase
{
    private readonly IPermapeopleApiService _permapeopleApiService;

    public PermapeopleTestController(IPermapeopleApiService permapeopleApiService)
    {
        _permapeopleApiService = permapeopleApiService;
    }

    // GET api/PermapeopleTest
    [HttpGet]
    public async Task<IActionResult> GetAllPlants([FromQuery] int? lastId = null)
    {
        var plants = await _permapeopleApiService.GetPlantsAsync(lastId);
        return Ok(plants);
    }

    // GET api/PermapeopleTest/101
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlant(int id)
    {
        var plant = await _permapeopleApiService.GetPlantByIdAsync(id);
        if (plant is null)
            return NotFound();

        return Ok(plant);
    }
}
