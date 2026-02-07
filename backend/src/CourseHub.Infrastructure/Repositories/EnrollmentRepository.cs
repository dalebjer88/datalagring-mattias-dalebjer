using CourseHub.Application.Enrollments;
using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public sealed class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(CourseHubDbContext db) : base(db)
    {
    }

    public Task<bool> ParticipantExistsAsync(int participantId, CancellationToken ct = default)
    {
        return Db.Participants.AnyAsync(x => x.Id == participantId, ct);
    }

    public Task<bool> CourseInstanceExistsAsync(int courseInstanceId, CancellationToken ct = default)
    {
        return Db.CourseInstances.AnyAsync(x => x.Id == courseInstanceId, ct);
    }

    public Task<bool> EnrollmentExistsAsync(int participantId, int courseInstanceId, CancellationToken ct = default)
    {
        return Db.Enrollments.AnyAsync(x => x.ParticipantId == participantId && x.CourseInstanceId == courseInstanceId, ct);
    }
}
