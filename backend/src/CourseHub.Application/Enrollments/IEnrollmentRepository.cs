using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Enrollments;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<bool> ParticipantExistsAsync(int participantId, CancellationToken ct = default);
    Task<bool> CourseInstanceExistsAsync(int courseInstanceId, CancellationToken ct = default);
    Task<bool> EnrollmentExistsAsync(int participantId, int courseInstanceId, CancellationToken ct = default);
}
