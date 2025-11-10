using Hortifia.Application.Posts.Commands.CreatePost;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        var newPostId = result.Value;
        return CreatedAtAction(nameof(CreatePost), new { newPostId }, new { newPostId });
    }
}
