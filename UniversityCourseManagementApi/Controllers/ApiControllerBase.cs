using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult OkResult<T>(T result)
    {
        return StatusCode(
            StatusCodes.Status200OK,
            ReturnResult<T>.Success(result, StatusCodes.Status200OK, HttpContext.TraceIdentifier));
    }

    protected IActionResult CreatedResult<T>(string actionName, object routeValues, T result)
    {
        var body = ReturnResult<T>.Success(result, StatusCodes.Status201Created, HttpContext.TraceIdentifier);
        return CreatedAtAction(actionName, routeValues, body);
    }

    protected IActionResult DeletedResult()
    {
        return StatusCode(
            StatusCodes.Status200OK,
            ReturnResult<object?>.Success(null, StatusCodes.Status200OK, HttpContext.TraceIdentifier));
    }
}
