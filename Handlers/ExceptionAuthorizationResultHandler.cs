using Kursplassen_Eksamen_Emne_3.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Kursplassen_Eksamen_Emne_3.Handlers;

public class ExceptionAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _default = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context,
        AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
            throw new UnauthorizedException("You must log in to do this.");

        if (authorizeResult.Forbidden)
            throw new ForbiddenException("You don't have permission to do this.");

        await _default.HandleAsync(next, context, policy, authorizeResult);
    }
}