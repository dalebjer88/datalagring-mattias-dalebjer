namespace CourseHub.Domain.Entities;

public class CourseInstance
{
    public int Id { get; private set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Capacity { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public int LocationId { get; set; }
    public Location? Location { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<CourseInstanceTeacher> CourseInstanceTeachers { get; set; } = new List<CourseInstanceTeacher>();
}
