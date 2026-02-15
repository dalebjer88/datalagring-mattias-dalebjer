using CourseHub.Application.CourseInstances;
using CourseHub.Application.Enrollments;
using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Registrations;

public sealed class RegistrationService : IRegistrationService
{
    private readonly ICourseInstanceRepository _courseInstanceRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly ITransactionRunner _tx;

    public RegistrationService(
        ICourseInstanceRepository courseInstanceRepo,
        IEnrollmentRepository enrollmentRepo,
        ITransactionRunner tx)
    {
        _courseInstanceRepo = courseInstanceRepo;
        _enrollmentRepo = enrollmentRepo;
        _tx = tx;
    }

    public Task<RegistrationResultDto> CreateCourseInstanceWithEnrollmentsAsync(
        CreateCourseInstanceWithEnrollmentsRequest request,
        CancellationToken ct = default)
    {
        Validate(request);

        return _tx.ExecuteAsync(async innerCt =>
        {
            if (!await _courseInstanceRepo.CourseExistsAsync(request.CourseId, innerCt))
                throw new InvalidOperationException("Course does not exist.");

            if (!await _courseInstanceRepo.LocationExistsAsync(request.LocationId, innerCt))
                throw new InvalidOperationException("Location does not exist.");

            var teacherIds = request.TeacherIds
                .Where(x => x > 0)
                .Distinct()
                .ToArray();

            if (teacherIds.Length == 0)
                throw new InvalidOperationException("At least one teacher is required.");

            if (!await _courseInstanceRepo.TeachersExistAsync(teacherIds, innerCt))
                throw new InvalidOperationException("One or more teachers do not exist.");

            var participantIds = request.ParticipantIds
                .Where(x => x > 0)
                .Distinct()
                .ToArray();

            if (participantIds.Length == 0)
                throw new InvalidOperationException("At least one participant is required.");

            foreach (var participantId in participantIds)
            {
                if (!await _enrollmentRepo.ParticipantExistsAsync(participantId, innerCt))
                    throw new InvalidOperationException("One or more participants do not exist.");
            }

            var courseInstance = new CourseInstance
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Capacity = request.Capacity,
                CourseId = request.CourseId,
                LocationId = request.LocationId
            };

            foreach (var teacherId in teacherIds)
            {
                courseInstance.CourseInstanceTeachers.Add(new CourseInstanceTeacher
                {
                    TeacherId = teacherId
                });
            }

            await _courseInstanceRepo.AddAsync(courseInstance, innerCt);

            foreach (var participantId in participantIds)
            {
                var enrollment = new Enrollment
                {
                    ParticipantId = participantId,
                    CourseInstance = courseInstance,
                    RegisteredAt = DateTime.UtcNow,
                    Status = request.Status.Trim()
                };

                await _enrollmentRepo.AddAsync(enrollment, innerCt);
            }

            await _courseInstanceRepo.SaveChangesAsync(innerCt);

            return new RegistrationResultDto(courseInstance.Id, participantIds.Length);
        }, ct);
    }

    private static void Validate(CreateCourseInstanceWithEnrollmentsRequest request)
    {
        if (request.CourseId <= 0) throw new InvalidOperationException("CourseId must be greater than 0.");
        if (request.LocationId <= 0) throw new InvalidOperationException("LocationId must be greater than 0.");
        if (request.Capacity <= 0) throw new InvalidOperationException("Capacity must be greater than 0.");
        if (request.EndDate < request.StartDate) throw new InvalidOperationException("End date cannot be before start date.");
        if (string.IsNullOrWhiteSpace(request.Status)) throw new InvalidOperationException("Status is required.");
        if (request.TeacherIds is null || request.TeacherIds.Length == 0) throw new InvalidOperationException("TeacherIds is required.");
        if (request.ParticipantIds is null || request.ParticipantIds.Length == 0) throw new InvalidOperationException("ParticipantIds is required.");
    }
}
