using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Include(enrollment => enrollment.Student)
            .Include(enrollment => enrollment.Course)
            .OrderBy(enrollment => enrollment.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Include(enrollment => enrollment.Student)
            .Include(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(enrollment => enrollment.Id == id, cancellationToken);
    }

    public Task<bool> ExistsForStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
    {
        return _context.Enrollments.AnyAsync(
            enrollment => enrollment.StudentId == studentId && enrollment.CourseId == courseId,
            cancellationToken);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
    }

    public void Update(Enrollment enrollment) => _context.Enrollments.Update(enrollment);

    public void Delete(Enrollment enrollment) => _context.Enrollments.Remove(enrollment);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
