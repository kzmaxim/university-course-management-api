namespace UniversityCourseManagementApi.DTO.Auth;

public class UserMeDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = [];
    public int? TeacherId { get; set; }
    public int? StudentId { get; set; }
}
