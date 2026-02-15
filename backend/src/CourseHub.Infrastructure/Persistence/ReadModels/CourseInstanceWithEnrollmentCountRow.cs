namespace CourseHub.Infrastructure.Persistence.ReadModels;

public sealed class CourseInstanceWithEnrollmentCountRow
{
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Capacity { get; set; }
    public int CourseId { get; set; }
    public int LocationId { get; set; }
    public int EnrollmentCount { get; set; }
}
