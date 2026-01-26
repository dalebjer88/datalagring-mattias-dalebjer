namespace CourseHub.Application.Courses;

public interface ICourseService
{
    Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken ct = default);
}
