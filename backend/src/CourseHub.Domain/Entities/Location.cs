namespace CourseHub.Domain.Entities;

public class Location
{
    public int Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<CourseInstance> CourseInstances { get; set; } = new List<CourseInstance>();
}
