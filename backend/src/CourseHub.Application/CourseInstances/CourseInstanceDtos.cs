namespace CourseHub.Application.CourseInstances;

public sealed record CreateCourseInstanceRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    int Capacity,
    int CourseId,
    int LocationId,
    int[] TeacherIds);

public sealed record UpdateCourseInstanceRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    int Capacity,
    int CourseId,
    int LocationId,
    int[] TeacherIds);

public sealed record CourseInstanceDto(
    int Id,
    DateOnly StartDate,
    DateOnly EndDate,
    int Capacity,
    int CourseId,
    int LocationId,
    int[] TeacherIds);
public sealed record CourseInstanceWithEnrollmentCountDto(
    int Id,
    DateOnly StartDate,
    DateOnly EndDate,
    int Capacity,
    int CourseId,
    int LocationId,
    int EnrollmentCount);

