using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Courses;

public class CourseUpdateDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Range(1, 10)]
    public int Credits { get; set; }

    [Range(1, int.MaxValue)]
    public int TeacherId { get; set; }
}
