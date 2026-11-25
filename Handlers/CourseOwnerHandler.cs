using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3.Handlers;

public class CourseOwnerRequirement : IAuthorizationRequirement { }

public class CourseOwnerHandler(KursplassenDbContext db)
    : AuthorizationHandler<CourseOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, CourseOwnerRequirement requirement)
    {
        if (!context.User.IsInRole("Instructor")) return;
        if (context.Resource is not HttpContext http) return;

        if (!Guid.TryParse(http.GetRouteValue("courseId")?.ToString(), out var courseId)) return;
        if (!Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return;

        var ownerId = await db.Courses
            .Where(c => c.Id == courseId)
            .Select(c => (Guid?)c.OwnerUserId)
            .FirstOrDefaultAsync();

        // Course doesn't exist: let the request through so the service returns 404
        if (ownerId is null || ownerId == userId)
            context.Succeed(requirement);
    }
}