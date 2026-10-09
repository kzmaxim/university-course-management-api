using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Enrollments;

public class EnrollmentUpdateDto
{
    [Required]
    [Range(0, 100)]
    public decimal Grade { get; set; }
}
