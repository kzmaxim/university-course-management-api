using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(ApplicationUser user, IEnumerable<string> roles);
}
