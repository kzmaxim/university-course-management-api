using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.Models;

public class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    [Range(0, 100)]
    public decimal? Grade { get; set; }

    public Student Student { get; set; } = null!;

    public Course Course { get; set; } = null!;
}
