using Hortifia.Application.Location.Dtos;
using Hortifia.Application.Location.Queries.GetCityName;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Controllers;

[ApiController]
[Route("api/location")]
[Authorize]
public class LocationController(IMediator mediator) : ControllerBase
{
    [HttpGet("city-name")]
    public async Task<ActionResult<CityNameDto>> GetCityName([FromQuery] GetCityNameQuery query)
    {
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }
}
