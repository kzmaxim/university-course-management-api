using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(enrollment => enrollment.Id);

        builder.Property(enrollment => enrollment.Grade)
            .HasPrecision(5, 2);

        builder.HasIndex(enrollment => new { enrollment.StudentId, enrollment.CourseId })
            .IsUnique();
    }
}
