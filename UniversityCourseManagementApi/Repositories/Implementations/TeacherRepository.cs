using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext _context;

    public TeacherRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Teachers
            .AsNoTracking()
            .OrderBy(teacher => teacher.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Teachers.FirstOrDefaultAsync(teacher => teacher.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Teachers.AnyAsync(teacher => teacher.Id == id, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return _context.Teachers.AnyAsync(
            teacher => teacher.Email == email && (!excludeId.HasValue || teacher.Id != excludeId.Value),
            cancellationToken);
    }

    public Task<bool> HasCoursesAsync(int teacherId, CancellationToken cancellationToken = default)
    {
        return _context.Courses.AnyAsync(course => course.TeacherId == teacherId, cancellationToken);
    }

    public async Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default)
    {
        await _context.Teachers.AddAsync(teacher, cancellationToken);
    }

    public void Update(Teacher teacher) => _context.Teachers.Update(teacher);

    public void Delete(Teacher teacher) => _context.Teachers.Remove(teacher);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
