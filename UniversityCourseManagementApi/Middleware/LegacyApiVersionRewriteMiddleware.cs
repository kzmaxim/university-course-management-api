namespace UniversityCourseManagementApi.Middleware;

/// <summary>
/// Maps unversioned /api/... requests to /api/v1/... so module 1 routes keep working.
/// </summary>
public class LegacyApiVersionRewriteMiddleware
{
    private readonly RequestDelegate _next;

    public LegacyApiVersionRewriteMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        if (path.StartsWithSegments("/api", out var remainder)
            && !path.StartsWithSegments("/api/v"))
        {
            context.Request.Path = new PathString("/api/v1").Add(remainder);
        }

        return _next(context);
    }
}
