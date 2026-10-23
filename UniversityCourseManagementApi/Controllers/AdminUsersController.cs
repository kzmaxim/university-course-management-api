using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Common.Exceptions;
using UniversityCourseManagementApi.DTO.Auth;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/admin/users")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminUsersController : ApiControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("roles")]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        if (!AppRoles.All.Contains(dto.Role))
        {
            throw new BadRequestException("INVALID_ROLE", "Unknown role");
        }

        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null)
        {
            throw new NotFoundException("USER_NOT_FOUND", "User not found");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in currentRoles)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
        }

        var assignResult = await _userManager.AddToRoleAsync(user, dto.Role);
        if (!assignResult.Succeeded)
        {
            var message = string.Join("; ", assignResult.Errors.Select(error => error.Description));
            throw new BadRequestException("ROLE_ASSIGNMENT_FAILED", message);
        }

        return OkResult(new { userId = user.Id, role = dto.Role });
    }
}
