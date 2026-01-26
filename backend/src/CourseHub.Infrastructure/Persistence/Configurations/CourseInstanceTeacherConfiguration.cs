using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public sealed class CourseInstanceTeacherConfiguration : IEntityTypeConfiguration<CourseInstanceTeacher>
{
    public void Configure(EntityTypeBuilder<CourseInstanceTeacher> builder)
    {
        builder.HasIndex(x => new { x.CourseInstanceId, x.TeacherId }).IsUnique();

        builder.HasOne(x => x.CourseInstance)
            .WithMany(x => x.CourseInstanceTeachers)
            .HasForeignKey(x => x.CourseInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.CourseInstanceTeachers)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
