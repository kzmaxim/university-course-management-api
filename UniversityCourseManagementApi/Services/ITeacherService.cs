using UniversityCourseManagementApi.DTO.Teachers;

namespace UniversityCourseManagementApi.Services;

public interface ITeacherService
{
    Task<IReadOnlyList<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TeacherDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TeacherDto> CreateAsync(TeacherCreateDto dto, CancellationToken cancellationToken = default);
    Task<TeacherDto> UpdateAsync(int id, TeacherUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
