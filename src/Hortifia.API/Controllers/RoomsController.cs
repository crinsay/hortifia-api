using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hortifia.Application.Rooms.Commands.CreateRoom;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Application.Rooms.Queries.GetRoomById;
using Hortifia.Application.Rooms.Commands.UpdateRoom;
using Hortifia.Application.Rooms.Queries.GetRooms;
using Hortifia.Application.Rooms.Commands.DeleteRoom;

namespace Hortifia.API.Controllers;

[ApiController]
[Authorize]
public class RoomsController(IMediator mediator) : ControllerBase
{
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command)
    {
        var result = await mediator.Send(command);

        return CreatedAtAction(nameof(GetRoomById), new { result.Value }, new { result.Value });
    }

    [HttpGet("rooms/{roomId}")]
    public async Task<ActionResult<RoomDto>> GetRoomById([FromRoute] int roomId)
    {
        var query = new GetRoomByIdQuery { RoomId = roomId };
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPatch("rooms/{roomId}")]
    public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomCommand command, [FromRoute] int roomId)
    {
        command.RoomId = roomId;
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpGet("rooms")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms([FromQuery] GetRoomsQuery query)
    {
        var result = await mediator.Send(query);

        return Ok(result.Value);
    }

    [HttpDelete("rooms/{roomId}")]
    public async Task<IActionResult> DeleteRoom([FromRoute] int roomId)
    {
        var command = new DeleteRoomCommand { RoomId = roomId };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }
}