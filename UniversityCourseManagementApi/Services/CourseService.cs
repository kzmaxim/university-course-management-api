using AutoMapper;
using UniversityCourseManagementApi.Common.Exceptions;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        ICourseRepository courseRepository,
        ITeacherRepository teacherRepository,
        IMapper mapper,
        ILogger<CourseService> logger)
    {
        _courseRepository = courseRepository;
        _teacherRepository = teacherRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CourseDto>> GetAllAsync(CourseFilterDto filter, CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetAllAsync(filter, cancellationToken);
        return _mapper.Map<IReadOnlyList<CourseDto>>(courses);
    }

    public async Task<CourseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            _logger.LogWarning("Attempt to get a non-existent course with id {CourseId}", id);
            throw new NotFoundException("COURSE_NOT_FOUND", "Course not found");
        }

        return _mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> CreateAsync(CourseCreateDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureTeacherExistsAsync(dto.TeacherId, cancellationToken);

        var course = _mapper.Map<Course>(dto);
        course.CreatedAt = DateTime.UtcNow;

        await _courseRepository.AddAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course created with id {CourseId}", course.Id);
        return await GetByIdAsync(course.Id, cancellationToken);
    }

    public async Task<CourseDto> UpdateAsync(int id, CourseUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            _logger.LogWarning("Attempt to update a non-existent course with id {CourseId}", id);
            throw new NotFoundException("COURSE_NOT_FOUND", "Course not found");
        }

        await EnsureTeacherExistsAsync(dto.TeacherId, cancellationToken);

        _mapper.Map(dto, course);
        _courseRepository.Update(course);
        await _courseRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course updated with id {CourseId}", id);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            _logger.LogWarning("Attempt to delete a non-existent course with id {CourseId}", id);
            throw new NotFoundException("COURSE_NOT_FOUND", "Course not found");
        }

        _courseRepository.Delete(course);
        await _courseRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Course deleted with id {CourseId}", id);
    }

    private async Task EnsureTeacherExistsAsync(int teacherId, CancellationToken cancellationToken)
    {
        if (!await _teacherRepository.ExistsAsync(teacherId, cancellationToken))
        {
            throw new NotFoundException("TEACHER_NOT_FOUND", "Course cannot reference a non-existent teacher");
        }
    }
}
