using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasIndex(x => new { x.ParticipantId, x.CourseInstanceId }).IsUnique();

        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.Property(x => x.RegisteredAt).IsRequired();

        builder.HasOne(x => x.Participant)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CourseInstance)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.CourseInstanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
