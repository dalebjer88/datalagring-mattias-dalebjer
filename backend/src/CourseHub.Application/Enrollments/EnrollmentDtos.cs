namespace CourseHub.Application.Enrollments;

public sealed record CreateEnrollmentRequest(int ParticipantId, int CourseInstanceId, string Status);
public sealed record UpdateEnrollmentRequest(string Status);
public sealed record EnrollmentDto(int Id, int ParticipantId, int CourseInstanceId, DateTime RegisteredAt, string status);