using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync(CourseFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(_context.Courses
            .AsNoTracking()
            .Include(course => course.Teacher), filter);

        return await query.OrderBy(course => course.Id).ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(course => course.Teacher)
            .FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetAllWithEnrollmentsAsync(
        CourseFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(_context.Courses
            .AsNoTracking()
            .Include(course => course.Teacher)
            .Include(course => course.Enrollments), filter);

        return await query.OrderBy(course => course.Id).ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetByIdWithEnrollmentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(course => course.Teacher)
            .Include(course => course.Enrollments)
            .FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }

    private static IQueryable<Course> ApplyFilter(IQueryable<Course> query, CourseFilterDto filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(course => course.Name.ToLower().Contains(search));
        }

        if (filter.TeacherId.HasValue)
        {
            query = query.Where(course => course.TeacherId == filter.TeacherId.Value);
        }

        if (filter.MinCredits.HasValue)
        {
            query = query.Where(course => course.Credits >= filter.MinCredits.Value);
        }

        if (filter.MaxCredits.HasValue)
        {
            query = query.Where(course => course.Credits <= filter.MaxCredits.Value);
        }

        return query;
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Courses.AnyAsync(course => course.Id == id, cancellationToken);
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _context.Courses.AddAsync(course, cancellationToken);
    }

    public void Update(Course course) => _context.Courses.Update(course);

    public void Delete(Course course) => _context.Courses.Remove(course);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
