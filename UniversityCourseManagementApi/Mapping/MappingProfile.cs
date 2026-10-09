using AutoMapper;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.DTO.Enrollments;
using UniversityCourseManagementApi.DTO.Students;
using UniversityCourseManagementApi.DTO.Teachers;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Enrollments));
        CreateMap<Enrollment, StudentCourseDto>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Course.Credits));
        CreateMap<StudentCreateDto, Student>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());
        CreateMap<StudentUpdateDto, Student>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());

        CreateMap<Teacher, TeacherDto>();
        CreateMap<TeacherCreateDto, Teacher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Courses, opt => opt.Ignore());
        CreateMap<TeacherUpdateDto, Teacher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Courses, opt => opt.Ignore());

        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.TeacherName,
                opt => opt.MapFrom(src => src.Teacher.FirstName + " " + src.Teacher.LastName));
        CreateMap<CourseCreateDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());
        CreateMap<CourseUpdateDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());

        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.StudentName,
                opt => opt.MapFrom(src => src.Student.FirstName + " " + src.Student.LastName))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));
        CreateMap<EnrollmentCreateDto, Enrollment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EnrollmentDate, opt => opt.Ignore())
            .ForMember(dest => dest.Grade, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
    }
}
