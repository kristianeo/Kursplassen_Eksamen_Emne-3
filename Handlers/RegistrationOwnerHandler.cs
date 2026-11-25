using System.Security.Claims;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3.Handlers;

public class RegistrationOwnerRequirement : IAuthorizationRequirement { }

public class RegistrationOwnerHandler(KursplassenDbContext db)
    : AuthorizationHandler<RegistrationOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, RegistrationOwnerRequirement requirement)
    {
        if (!context.User.IsInRole("Participant")) return;
        if (context.Resource is not HttpContext http) return;

        if (!Guid.TryParse(http.GetRouteValue("registrationId")?.ToString(), out var registrationId)) return;
        if (!Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return;

        var participantId = await db.Registrations
            .Where(r => r.Id == registrationId)
            .Select(c => (Guid?)c.ParticipantUserId)
            .FirstOrDefaultAsync();

        if (participantId is null)
            throw new NotFoundException("Course not found.");
        
        if (participantId == userId)
            context.Succeed(requirement);
    }
}