using System.Security.Claims;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kursplassen_Eksamen_Emne_3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController(IRegistrationService service) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpPost("/api/courses/{courseId:Guid}/registrations")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType<RegistrationDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegistrationDto>> Register(Guid courseId)
    {
        var created = await service.Create(courseId, CurrentUserId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserRegistrationDto>>> GetUserRegistrations()
    {
        return Ok(await service.GetUserRegistrations(CurrentUserId));
    }
    
    [HttpDelete("{registrationId:Guid}")]
    [Authorize(Policy = "RegistrationOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Cancel(Guid registrationId)
    {
        await service.Cancel(registrationId);
        return NoContent();
    }
}