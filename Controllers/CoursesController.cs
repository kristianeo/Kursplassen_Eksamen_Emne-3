using System.Security.Claims;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kursplassen_Eksamen_Emne_3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseSummaryDto>>> GetAll()
    {
        return Ok(await service.GetAll());
    }

    [HttpGet("{courseId:Guid}")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetById(Guid courseId)
    {
        return Ok(await service.GetById(courseId));
    }

    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType<CourseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CourseDto>> Create(CreateCourseDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var created = await service.Create(dto, userId);
        return CreatedAtAction(nameof(GetById), new {courseId = created.Id },
            created);
    }
    
    [HttpPut("{courseId:Guid}")]
    [Authorize(Policy = "CourseOwner")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CourseDto>> EditCourse(EditCourseDto dto, Guid courseId)
    {
        return Ok(await service.Edit(dto, courseId));
    }

    [HttpDelete("{courseId:Guid}")]
    [Authorize(Policy = "CourseOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete(Guid courseId)
    {
        await service.Delete(courseId);
        return NoContent();
    }
}