using Blog.Application.Dto;
using Blog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        var userId = GetUserId();

        var post = await _postService.CreatePostAsync(
            userId,
            dto.Title,
            dto.Content,
            dto.TagIds
        );

        return Ok(post);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetMyPosts()
    {
        var userId = GetUserId();
        var posts = await _postService.GetPostsByUserAsync(userId);
        return Ok(posts);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _postService.GetAllPostsAsync());
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetById(Guid postId)
    {
        return Ok(await _postService.GetPostByIdAsync(postId));
    }

    [HttpPut("{postId}")]
    public async Task<IActionResult> Update(Guid postId, [FromBody] UpdatePostDto dto)
    {
        var result = await _postService.UpdatePostAsync(
            postId,
            dto.Title,
            dto.Content,
            dto.TagIds
        );

        return Ok(result);
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> Delete(Guid postId)
    {
        var result = await _postService.DeletePostAsync(postId);
        return Ok(result);
    }


    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");

        return Guid.Parse(userId);
    }
}
