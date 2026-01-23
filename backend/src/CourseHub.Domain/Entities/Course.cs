namespace CourseHub.Domain.Entities;

public class Course
{
    public int Id { get; private set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<CourseInstance> CourseInstances { get; set; } = new List<CourseInstance>();
}
