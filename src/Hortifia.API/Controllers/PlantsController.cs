using Hortifia.Application.Plants.Commands.CreatePlant;
using Hortifia.Application.Plants.Queries.GetPlantById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class PlantsController(IMediator mediator) : ControllerBase
{
    [HttpPost("plants")]
    public async Task<IActionResult> CreatePlant([FromForm] CreatePlantCommand command)
    {
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetPlantById), new { plantId = result.Value }, new { result.Value });
    }

    [HttpGet("plants/{plantId}")]
    public async Task<IActionResult> GetPlantById([FromRoute] int plantId)
    {
        var query = new GetPlantByIdQuery { PlantId = plantId };
        var result = await mediator.Send(query);

        if (!result.IsSuccess) 
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }
}
