namespace CourseHub.Domain.Entities;

public class Teacher
{
    public int Id { get; private set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Expertise { get; set; } = string.Empty;
    public ICollection<CourseInstanceTeacher> CourseInstanceTeachers { get; set; } = new List<CourseInstanceTeacher>();
}
