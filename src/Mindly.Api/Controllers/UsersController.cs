using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mindly.Api.Representations;
using Mindly.Application.DTOs.Users;
using Mindly.Application.Services.Contracts;

namespace Mindly.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserProfileService _userService;

    public UsersController(IUserProfileService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<Resource<UserProfileDto>>> CreateAsync(
        [FromBody] CreateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = user.Id }, ToResource(user));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Resource<UserProfileDto>>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(ToResource(user));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Resource<UserProfileDto>>> UpdateAsync(
        Guid id,
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateAsync(id, request, cancellationToken);
        return Ok(ToResource(user));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private Resource<UserProfileDto> ToResource(UserProfileDto user)
    {
        var links = new List<LinkDto>
        {
            new(Url.Action(nameof(GetByIdAsync), new { id = user.Id })!, "self", HttpMethods.Get),
            new(Url.Action(nameof(UpdateAsync), new { id = user.Id })!, "update", HttpMethods.Put),
            new(Url.Action(nameof(DeleteAsync), new { id = user.Id })!, "delete", HttpMethods.Delete)
        };

        return new Resource<UserProfileDto>
        {
            Data = user,
            Links = links
        };
    }
}

