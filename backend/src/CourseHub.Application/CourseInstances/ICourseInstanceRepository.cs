using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.CourseInstances;

public interface ICourseInstanceRepository : IBaseRepository<CourseInstance>
{
    Task<bool> CourseExistsAsync(int courseId, CancellationToken ct = default);
    Task<bool> LocationExistsAsync(int locationId, CancellationToken ct = default);
}
