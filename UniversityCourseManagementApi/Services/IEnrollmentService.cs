using UniversityCourseManagementApi.DTO.Enrollments;

namespace UniversityCourseManagementApi.Services;

public interface IEnrollmentService
{
    Task<IReadOnlyList<EnrollmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EnrollmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EnrollmentDto> EnrollAsync(EnrollmentCreateDto dto, CancellationToken cancellationToken = default);
    Task<EnrollmentDto> UpdateGradeAsync(int id, EnrollmentUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
