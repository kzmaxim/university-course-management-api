using AutoMapper;
using UniversityCourseManagementApi.Common.Exceptions;
using UniversityCourseManagementApi.DTO.Students;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentService> _logger;

    public StudentService(IStudentRepository studentRepository, IMapper mapper, ILogger<StudentService> logger)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var students = await _studentRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StudentDto>>(students);
    }

    public async Task<StudentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdWithCoursesAsync(id, cancellationToken);
        if (student is null)
        {
            _logger.LogWarning("Attempt to get a non-existent student with id {StudentId}", id);
            throw new NotFoundException("STUDENT_NOT_FOUND", "Student not found");
        }

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> CreateAsync(StudentCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (await _studentRepository.EmailExistsAsync(dto.Email, cancellationToken: cancellationToken))
        {
            throw new ConflictException("STUDENT_EMAIL_EXISTS", "A student with this email already exists");
        }

        var student = _mapper.Map<Student>(dto);
        student.CreatedAt = DateTime.UtcNow;

        await _studentRepository.AddAsync(student, cancellationToken);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Student created with id {StudentId}", student.Id);
        return await GetByIdAsync(student.Id, cancellationToken);
    }

    public async Task<StudentDto> UpdateAsync(int id, StudentUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            _logger.LogWarning("Attempt to update a non-existent student with id {StudentId}", id);
            throw new NotFoundException("STUDENT_NOT_FOUND", "Student not found");
        }

        if (await _studentRepository.EmailExistsAsync(dto.Email, id, cancellationToken))
        {
            throw new ConflictException("STUDENT_EMAIL_EXISTS", "A student with this email already exists");
        }

        _mapper.Map(dto, student);
        _studentRepository.Update(student);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Student updated with id {StudentId}", id);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            _logger.LogWarning("Attempt to delete a non-existent student with id {StudentId}", id);
            throw new NotFoundException("STUDENT_NOT_FOUND", "Student not found");
        }

        _studentRepository.Delete(student);
        await _studentRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Student deleted with id {StudentId}", id);
    }
}
