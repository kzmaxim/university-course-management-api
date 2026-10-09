using UniversityCourseManagementApi.DTO.Courses;

namespace UniversityCourseManagementApi.Services;

public interface ICourseService
{
    Task<IReadOnlyList<CourseDto>> GetAllAsync(CourseFilterDto filter, CancellationToken cancellationToken = default);
    Task<CourseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseDto> CreateAsync(CourseCreateDto dto, CancellationToken cancellationToken = default);
    Task<CourseDto> UpdateAsync(int id, CourseUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
