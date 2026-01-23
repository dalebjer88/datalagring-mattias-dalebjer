namespace CourseHub.Domain.Entities;

public class Enrollment
{
    public int Id { get; private set; }
    public int ParticipantId { get; set; }
    public Participant? Participant { get; set; }
    public int CourseInstanceId { get; set; }
    public CourseInstance? CourseInstance { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
