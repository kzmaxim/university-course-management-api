using UniversityCourseManagementApi.DTO.Students;

namespace UniversityCourseManagementApi.Services;

public interface IStudentService
{
    Task<IReadOnlyList<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateAsync(StudentCreateDto dto, CancellationToken cancellationToken = default);
    Task<StudentDto> UpdateAsync(int id, StudentUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
