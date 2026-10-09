using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(student => student.Id);
            entity.HasIndex(student => student.Email).IsUnique();
            entity.HasMany(student => student.Enrollments)
                .WithOne(enrollment => enrollment.Student)
                .HasForeignKey(enrollment => enrollment.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(teacher => teacher.Id);
            entity.HasIndex(teacher => teacher.Email).IsUnique();
        });

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        var createdAt = new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Teacher>().HasData(
            new Teacher
            {
                Id = 1,
                FirstName = "Elena",
                LastName = "Ivanova",
                Email = "elena.ivanova@university.kz",
                Department = "Software Engineering"
            },
            new Teacher
            {
                Id = 2,
                FirstName = "Marat",
                LastName = "Suleimenov",
                Email = "marat.suleimenov@university.kz",
                Department = "Information Systems"
            });

        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                FirstName = "Alex",
                LastName = "Smith",
                Email = "alex.smith@student.kz",
                BirthDate = new DateOnly(2004, 3, 12),
                CreatedAt = createdAt
            },
            new Student
            {
                Id = 2,
                FirstName = "Aigerim",
                LastName = "Nurpeisova",
                Email = "aigerim.nurpeisova@student.kz",
                BirthDate = new DateOnly(2003, 11, 2),
                CreatedAt = createdAt
            },
            new Student
            {
                Id = 3,
                FirstName = "Daniyar",
                LastName = "Omarov",
                Email = "daniyar.omarov@student.kz",
                BirthDate = new DateOnly(2005, 7, 21),
                CreatedAt = createdAt
            });

        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                Id = 1,
                Name = "Web Services Development",
                Description = "REST APIs with ASP.NET Core",
                Credits = 5,
                TeacherId = 1,
                CreatedAt = createdAt
            },
            new Course
            {
                Id = 2,
                Name = "Database Systems",
                Description = "Relational databases and ORM",
                Credits = 4,
                TeacherId = 2,
                CreatedAt = createdAt
            },
            new Course
            {
                Id = 3,
                Name = "Software Architecture",
                Description = "Layered architecture and design patterns",
                Credits = 3,
                TeacherId = 1,
                CreatedAt = createdAt
            });

        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                Id = 1,
                StudentId = 1,
                CourseId = 1,
                EnrollmentDate = createdAt,
                Grade = 87.5m
            },
            new Enrollment
            {
                Id = 2,
                StudentId = 2,
                CourseId = 1,
                EnrollmentDate = createdAt,
                Grade = null
            });
    }
}
