using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Enrollments;

public class EnrollmentCreateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }
}
