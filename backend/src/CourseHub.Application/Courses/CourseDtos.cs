namespace CourseHub.Application.Courses;

public sealed record CreateCourseRequest(string CourseCode, string Title, string Description);

public sealed record CourseDto(int Id, string CourseCode, string Title, string Description);
