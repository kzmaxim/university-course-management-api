using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Common.Exceptions;
using UniversityCourseManagementApi.DTO.Auth;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ApiControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<UserMeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            var message = string.Join("; ", createResult.Errors.Select(error => error.Description));
            throw new BadRequestException("REGISTRATION_FAILED", message);
        }

        await _userManager.AddToRoleAsync(user, AppRoles.Student);

        var me = await BuildUserMeDtoAsync(user);
        return CreatedResult(nameof(Me), null!, me);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user is null)
        {
            throw new UnauthorizedAppException("AUTH_FAILED", "Invalid username or password");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            throw new UnauthorizedAppException("ACCOUNT_LOCKED", "Account is temporarily locked due to failed login attempts");
        }

        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAppException("AUTH_FAILED", "Invalid username or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.CreateAccessToken(user, roles);

        return OkResult(new AuthResponseDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAt
        });
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ReturnResult<UserMeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            throw new UnauthorizedAppException("AUTH_FAILED", "User not found");
        }

        return OkResult(await BuildUserMeDtoAsync(user));
    }

    private async Task<UserMeDto> BuildUserMeDtoAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserMeDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            TeacherId = user.TeacherId,
            StudentId = user.StudentId
        };
    }
}
