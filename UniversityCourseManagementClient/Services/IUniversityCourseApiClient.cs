using UniversityCourseManagementClient.Models;

namespace UniversityCourseManagementClient.Services;

public interface IUniversityCourseApiClient
{
    Task<(IReadOnlyList<CourseListItem> Courses, string? Error)> GetCoursesAsync(CancellationToken cancellationToken = default);
    Task<(CourseListItem? Course, string? Error)> GetCourseAsync(int id, CancellationToken cancellationToken = default);
}
