namespace CourseHub.Domain.Entities;

public class Participant
{
    public int Id { get; private set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
