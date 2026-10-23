using AutoMapper;
using UniversityCourseManagementApi.Common.Exceptions;
using UniversityCourseManagementApi.DTO.Enrollments;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IMapper mapper,
        ILogger<EnrollmentService> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<EnrollmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var enrollments = await _enrollmentRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EnrollmentDto>>(enrollments);
    }

    public async Task<EnrollmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id, cancellationToken);
        if (enrollment is null)
        {
            _logger.LogWarning("Attempt to get a non-existent enrollment with id {EnrollmentId}", id);
            throw new NotFoundException("ENROLLMENT_NOT_FOUND", "Enrollment not found");
        }

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<EnrollmentDto> EnrollAsync(EnrollmentCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _studentRepository.ExistsAsync(dto.StudentId, cancellationToken))
        {
            _logger.LogWarning(
                "Cannot enroll: student {StudentId} does not exist",
                dto.StudentId);
            throw new NotFoundException("STUDENT_NOT_FOUND", "Cannot enroll a non-existent student");
        }

        if (!await _courseRepository.ExistsAsync(dto.CourseId, cancellationToken))
        {
            _logger.LogWarning(
                "Cannot enroll: course {CourseId} does not exist",
                dto.CourseId);
            throw new NotFoundException("COURSE_NOT_FOUND", "Cannot enroll a student on a non-existent course");
        }

        if (await _enrollmentRepository.ExistsForStudentAndCourseAsync(dto.StudentId, dto.CourseId, cancellationToken))
        {
            _logger.LogWarning(
                "Duplicate enrollment: student {StudentId} is already enrolled in course {CourseId}",
                dto.StudentId,
                dto.CourseId);
            throw new ConflictException("ENROLLMENT_ALREADY_EXISTS", "The student is already enrolled in this course");
        }

        var enrollment = _mapper.Map<Enrollment>(dto);
        enrollment.EnrollmentDate = DateTime.UtcNow;

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Student {StudentId} enrolled in course {CourseId} with enrollment id {EnrollmentId}",
            dto.StudentId,
            dto.CourseId,
            enrollment.Id);

        return await GetByIdAsync(enrollment.Id, cancellationToken);
    }

    public async Task<EnrollmentDto> UpdateGradeAsync(int id, EnrollmentUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id, cancellationToken);
        if (enrollment is null)
        {
            _logger.LogWarning("Attempt to update a non-existent enrollment with id {EnrollmentId}", id);
            throw new NotFoundException("ENROLLMENT_NOT_FOUND", "Enrollment not found");
        }

        enrollment.Grade = dto.Grade;
        _enrollmentRepository.Update(enrollment);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Enrollment grade updated for id {EnrollmentId}", id);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id, cancellationToken);
        if (enrollment is null)
        {
            _logger.LogWarning("Attempt to delete a non-existent enrollment with id {EnrollmentId}", id);
            throw new NotFoundException("ENROLLMENT_NOT_FOUND", "Enrollment not found");
        }

        _enrollmentRepository.Delete(enrollment);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Enrollment deleted with id {EnrollmentId}", id);
    }
}
