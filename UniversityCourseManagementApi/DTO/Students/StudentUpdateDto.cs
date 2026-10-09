using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Students;

public class StudentUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; set; }
}
