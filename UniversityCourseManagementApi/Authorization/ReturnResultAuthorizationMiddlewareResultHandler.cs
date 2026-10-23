using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using UniversityCourseManagementApi.Common;

namespace UniversityCourseManagementApi.Authorization;

public class ReturnResultAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ReturnResult<object>.Fail(
                StatusCodes.Status401Unauthorized,
                "UNAUTHORIZED",
                "Authentication is required",
                context.TraceIdentifier));
            return;
        }

        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ReturnResult<object>.Fail(
                StatusCodes.Status403Forbidden,
                "FORBIDDEN",
                "You do not have permission to perform this action",
                context.TraceIdentifier));
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
