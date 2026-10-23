using System.Security.Claims;
using UniversityCourseManagementApi.Common;

namespace UniversityCourseManagementApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public bool IsInRole(string role) => User?.IsInRole(role) == true;

    public int? TeacherId => ParseNullableInt(User?.FindFirstValue(AppClaims.TeacherId));

    public int? StudentId => ParseNullableInt(User?.FindFirstValue(AppClaims.StudentId));

    private static int? ParseNullableInt(string? value) =>
        int.TryParse(value, out var id) ? id : null;
}
