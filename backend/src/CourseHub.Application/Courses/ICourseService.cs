namespace CourseHub.Application.Courses;

public interface ICourseService
{
    Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken ct = default);
    Task<CourseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CourseDto?> UpdateAsync(int id, UpdateCourseRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
