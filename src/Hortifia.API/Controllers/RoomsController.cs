using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hortifia.Application.Rooms.Commands.CreateRoom;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Application.Rooms.Queries.GetRoomById;

namespace Hortifia.API.Controllers;

[ApiController]
[Authorize]
public class RoomsController(IMediator mediator) : ControllerBase
{
    [HttpPost("rooms")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command)
    {
        var roomId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetRoomById), new { roomId }, new { roomId });
    }

    [HttpGet("rooms/{roomId}")]
    public async Task<ActionResult<RoomDto>> GetRoomById([FromRoute] int roomId)
    {
        var query = new GetRoomByIdQuery { RoomId = roomId };
        var roomResult = await mediator.Send(query);

        return Ok(roomResult.Value);
    }
}

