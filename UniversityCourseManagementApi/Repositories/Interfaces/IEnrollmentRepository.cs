using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Enrollment>> GetAllForStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Enrollment>> GetAllForTeacherAsync(int teacherId, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsForStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    void Update(Enrollment enrollment);
    void Delete(Enrollment enrollment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
