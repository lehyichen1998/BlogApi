using Blog.Application.Dto;
using Blog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly UserProfileService _userProfileService;

    public ProfileController(UserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetProfile([FromQuery] string UserId)
    {
        if (string.IsNullOrEmpty(UserId))
            return BadRequest("UserId is required.");

        var profile = await _userProfileService.GetProfileAsync(Guid.Parse(UserId));
        return Ok(profile);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var result = await _userProfileService.UpdateProfileAsync(dto);
        return Ok(new { message = result });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllProfiles()
    {
        var profiles = await _userProfileService.GetAllUsersAsync();
        return Ok(profiles);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteProfile([FromQuery] string UserId)
    {
        if (string.IsNullOrEmpty(UserId))
            return BadRequest("UserId is required.");
        var result = await _userProfileService.DeleteProfileAsync(Guid.Parse(UserId));
        return Ok(new { message = result });
    }
}
