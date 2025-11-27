using Hortifia.Application.Weather.Queries.GetCurrentWeather;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Controllers;

[ApiController]
[Route("api/weather")]
[Authorize]
public class WeatherController(IMediator mediator) : ControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentWeather()
    {
        var query = new GetCurrentWeatherQuery();
        var result = await mediator.Send(query);

        return Ok(result.Value);
    }
}
