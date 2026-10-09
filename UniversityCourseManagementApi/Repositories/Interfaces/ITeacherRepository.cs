using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Repositories.Interfaces;

public interface ITeacherRepository
{
    Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasCoursesAsync(int teacherId, CancellationToken cancellationToken = default);
    Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default);
    void Update(Teacher teacher);
    void Delete(Teacher teacher);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
