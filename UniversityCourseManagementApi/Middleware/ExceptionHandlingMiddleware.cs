using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Common.Exceptions;

namespace UniversityCourseManagementApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException exception)
        {
            _logger.LogWarning(
                exception,
                "Business error {ErrorCode} on {Path}",
                exception.ErrorCode,
                context.Request.Path);

            await WriteAsync(context, ReturnResult<object>.Fail(
                exception.StatusCode,
                exception.ErrorCode,
                exception.Message,
                context.TraceIdentifier));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);

            await WriteAsync(context, ReturnResult<object>.Fail(
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "An unexpected error occurred",
                context.TraceIdentifier));
        }
    }

    private static async Task WriteAsync(HttpContext context, ReturnResult<object> result)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = result.StatusCode;
        await context.Response.WriteAsJsonAsync(result);
    }
}
