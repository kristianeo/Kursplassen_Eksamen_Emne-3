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
    
    [HttpPost("/api/courses/{courseId:Guid}/registration")]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<RegistrationDto>> Register(Guid courseId)
    {
        var created = await service.Create(courseId, CurrentUserId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetUserRegistrations()
    {
        return Ok(await service.GetUserRegistrations(CurrentUserId));
    }
    
    [HttpDelete("{registrationId:Guid}")]
    [Authorize(Policy = "RegistrationOwner")]
    public async Task<ActionResult> Cancel(Guid registrationId)
    {
        await service.Cancel(registrationId);
        return NoContent();
    }
}