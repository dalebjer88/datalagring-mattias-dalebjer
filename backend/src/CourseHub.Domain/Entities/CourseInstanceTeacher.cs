namespace CourseHub.Domain.Entities;

public class CourseInstanceTeacher
{
    public int Id { get; private set; }
    public int CourseInstanceId { get; set; }
    public CourseInstance? CourseInstance { get; set; }
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}
