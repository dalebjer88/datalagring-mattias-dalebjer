namespace CourseHub.Application.Registrations;

public sealed record CreateCourseInstanceWithEnrollmentsRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    int Capacity,
    int CourseId,
    int LocationId,
    int[] TeacherIds,
    int[] ParticipantIds,
    string Status);

public sealed record RegistrationResultDto(
    int CourseInstanceId,
    int EnrollmentCount);
