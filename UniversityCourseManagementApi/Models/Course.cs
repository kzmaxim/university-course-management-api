using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Range(1, 10)]
    public int Credits { get; set; }

    public int TeacherId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
