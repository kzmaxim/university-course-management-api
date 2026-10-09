using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .AsNoTracking()
            .Include(student => student.Enrollments)
            .ThenInclude(enrollment => enrollment.Course)
            .OrderBy(student => student.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Students.FirstOrDefaultAsync(student => student.Id == id, cancellationToken);
    }

    public async Task<Student?> GetByIdWithCoursesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .AsNoTracking()
            .Include(student => student.Enrollments)
            .ThenInclude(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(student => student.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Students.AnyAsync(student => student.Id == id, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return _context.Students.AnyAsync(
            student => student.Email == email && (!excludeId.HasValue || student.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        await _context.Students.AddAsync(student, cancellationToken);
    }

    public void Update(Student student) => _context.Students.Update(student);

    public void Delete(Student student) => _context.Students.Remove(student);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
