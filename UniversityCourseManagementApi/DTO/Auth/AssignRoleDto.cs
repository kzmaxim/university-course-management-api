using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Auth;

public class AssignRoleDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}
