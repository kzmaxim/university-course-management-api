using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(course => course.Id);

        builder.Property(course => course.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(course => course.Description)
            .HasMaxLength(1000);

        builder.Property(course => course.Credits)
            .IsRequired();

        builder.HasOne(course => course.Teacher)
            .WithMany(teacher => teacher.Courses)
            .HasForeignKey(course => course.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(course => course.Enrollments)
            .WithOne(enrollment => enrollment.Course)
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
