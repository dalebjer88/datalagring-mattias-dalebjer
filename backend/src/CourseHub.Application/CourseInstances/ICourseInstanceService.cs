namespace CourseHub.Application.CourseInstances;

public interface ICourseInstanceService
{
    Task<CourseInstanceDto> CreateAsync(CreateCourseInstanceRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CourseInstanceDto>> GetAllAsync(CancellationToken ct = default);
    Task<CourseInstanceDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CourseInstanceDto?> UpdateAsync(int id, UpdateCourseInstanceRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
