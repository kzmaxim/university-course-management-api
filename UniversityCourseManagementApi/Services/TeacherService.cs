using AutoMapper;
using UniversityCourseManagementApi.Common.Exceptions;
using UniversityCourseManagementApi.DTO.Teachers;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TeacherService> _logger;

    public TeacherService(ITeacherRepository teacherRepository, IMapper mapper, ILogger<TeacherService> logger)
    {
        _teacherRepository = teacherRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var teachers = await _teacherRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TeacherDto>>(teachers);
    }

    public async Task<TeacherDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);
        if (teacher is null)
        {
            _logger.LogWarning("Attempt to get a non-existent teacher with id {TeacherId}", id);
            throw new NotFoundException("TEACHER_NOT_FOUND", "Teacher not found");
        }

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> CreateAsync(TeacherCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (await _teacherRepository.EmailExistsAsync(dto.Email, cancellationToken: cancellationToken))
        {
            throw new ConflictException("TEACHER_EMAIL_EXISTS", "A teacher with this email already exists");
        }

        var teacher = _mapper.Map<Teacher>(dto);
        await _teacherRepository.AddAsync(teacher, cancellationToken);
        await _teacherRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Teacher created with id {TeacherId}", teacher.Id);
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> UpdateAsync(int id, TeacherUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);
        if (teacher is null)
        {
            _logger.LogWarning("Attempt to update a non-existent teacher with id {TeacherId}", id);
            throw new NotFoundException("TEACHER_NOT_FOUND", "Teacher not found");
        }

        if (await _teacherRepository.EmailExistsAsync(dto.Email, id, cancellationToken))
        {
            throw new ConflictException("TEACHER_EMAIL_EXISTS", "A teacher with this email already exists");
        }

        _mapper.Map(dto, teacher);
        _teacherRepository.Update(teacher);
        await _teacherRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Teacher updated with id {TeacherId}", id);
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);
        if (teacher is null)
        {
            _logger.LogWarning("Attempt to delete a non-existent teacher with id {TeacherId}", id);
            throw new NotFoundException("TEACHER_NOT_FOUND", "Teacher not found");
        }

        if (await _teacherRepository.HasCoursesAsync(id, cancellationToken))
        {
            throw new ConflictException("TEACHER_HAS_COURSES", "Cannot delete a teacher who is assigned to courses");
        }

        _teacherRepository.Delete(teacher);
        await _teacherRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Teacher deleted with id {TeacherId}", id);
    }
}
