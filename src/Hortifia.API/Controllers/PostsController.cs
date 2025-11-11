using Hortifia.Application.Posts.Commands.CreatePost;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Application.Posts.Queries.GetPostById;
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

        var postId = result.Value;
        return CreatedAtAction(nameof(GetPostById), new { postId }, new { postId });
    }

    [HttpGet("{postId}")]
    public async Task<ActionResult<PostDto>> GetPostById([FromRoute] int postId)
    {
        var query = new GetPostByIdQuery { PostId = postId };

        var result = await mediator.Send(query);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return Ok(result.Value);
    }
}
