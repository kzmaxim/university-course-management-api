namespace UniversityCourseManagementApi.DTO.Students;

public class StudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<StudentCourseDto> Courses { get; set; } = [];
}

public class StudentCourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int Credits { get; set; }
    public decimal? Grade { get; set; }
    public DateTime EnrollmentDate { get; set; }
}
