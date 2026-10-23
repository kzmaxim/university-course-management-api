using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<IReadOnlyList<Course>> GetAllAsync(CourseFilterDto filter, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetAllWithEnrollmentsAsync(CourseFilterDto filter, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdWithEnrollmentsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
    void Update(Course course);
    void Delete(Course course);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
