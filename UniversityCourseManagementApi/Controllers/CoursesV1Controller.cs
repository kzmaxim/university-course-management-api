using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
public class CoursesV1Controller : ApiControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesV1Controller(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CourseFilterDto filter, CancellationToken cancellationToken)
    {
        return OkResult(await _courseService.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        return OkResult(await _courseService.GetByIdAsync(id, cancellationToken));
    }
}
