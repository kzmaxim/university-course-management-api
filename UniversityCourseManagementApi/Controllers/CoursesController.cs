using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/courses")]
public class CoursesController : ApiControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllV1([FromQuery] CourseFilterDto filter, CancellationToken cancellationToken)
    {
        return OkResult(await _courseService.GetAllAsync(filter, cancellationToken));
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<CourseV2Dto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllV2([FromQuery] CourseFilterDto filter, CancellationToken cancellationToken)
    {
        return OkResult(await _courseService.GetAllV2Async(filter, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdV1(int id, CancellationToken cancellationToken)
    {
        return OkResult(await _courseService.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [MapToApiVersion("2.0")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<CourseV2Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdV2(int id, CancellationToken cancellationToken)
    {
        return OkResult(await _courseService.GetByIdV2Async(id, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CourseCreateDto dto, CancellationToken cancellationToken)
    {
        var course = await _courseService.CreateAsync(dto, cancellationToken);
        return CreatedResult(nameof(GetByIdV1), new { id = course.Id, version = "1.0" }, course);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] CourseUpdateDto dto, CancellationToken cancellationToken)
    {
        return OkResult(await _courseService.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _courseService.DeleteAsync(id, cancellationToken);
        return DeletedResult();
    }
}
