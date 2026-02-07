namespace CourseHub.Application.Teachers;

public sealed record CreateTeacherRequest(string Email, string FirstName, string LastName, string Expertise);

public sealed record UpdateTeacherRequest(string Email, string FirstName, string LastName, string Expertise);

public sealed record TeacherDto(int Id, string Email, string FirstName, string LastName, string Expertise);
