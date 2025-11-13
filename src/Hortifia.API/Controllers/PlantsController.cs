using Hortifia.Application.Plants.Commands.CreatePlant;
using Hortifia.Application.Plants.Commands.DeletePlant;
using Hortifia.Application.Plants.Commands.UpdateIsFavourite;
using Hortifia.Application.Plants.Commands.UpdatePlant;
using Hortifia.Application.Plants.Commands.WaterPlant;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Plants.Queries.GetPlantById;
using Hortifia.Application.Plants.Queries.GetPlants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Controllers;

[ApiController]
[Route("api/plants")]
[Authorize]
public class PlantsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePlant([FromForm] CreatePlantCommand command)
    {
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetPlantById), new { plantId = result.Value }, new { result.Value });
    }

    [HttpGet("{plantId}")]
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

    [HttpPatch("{plantId}")]
    public async Task<IActionResult> UpdatePlant([FromForm] UpdatePlantCommand command, [FromRoute] int plantId)
    {
        command.PlantId = plantId;
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpPatch("{plantId}/favourite")]
    public async Task<IActionResult> UpdateIsFavourite([FromRoute] int plantId)
    {
        var command = new UpdateIsFavouriteCommand { PlantId = plantId };
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpPatch("{plantId}/water")]
    public async Task<IActionResult> WaterPlant([FromRoute] int plantId)
    {
        var command = new WaterPlantCommand { PlantId = plantId };
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpDelete("{plantId}")]
    public async Task<IActionResult> DeletePlant([FromRoute] int plantId)
    {
        var command = new DeletePlantCommand { PlantId = plantId };
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantListDto>>> GetPlants([FromQuery] GetPlantsQuery query)
    {
        var result = await mediator.Send(query);

        return Ok(result.Value);
    }
}
