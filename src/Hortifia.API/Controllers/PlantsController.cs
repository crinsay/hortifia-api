using Hortifia.Application.Plants.Commands.CreatePlant;
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

        return Ok(result);
    }
}
