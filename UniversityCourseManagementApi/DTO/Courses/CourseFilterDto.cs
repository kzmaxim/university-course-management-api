namespace UniversityCourseManagementApi.DTO.Courses;

public class CourseFilterDto
{
    public string? Search { get; set; }
    public int? TeacherId { get; set; }
    public int? MinCredits { get; set; }
    public int? MaxCredits { get; set; }
}
