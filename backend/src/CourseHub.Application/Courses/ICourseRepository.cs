using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Courses;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<bool> CourseCodeExistsAsync(string courseCode, CancellationToken ct = default);
    Task<Course?> GetByCourseCodeAsync(string courseCode, CancellationToken ct = default);
    Task<bool> IsUsedByCourseInstancesAsync(int courseId, CancellationToken ct = default);
    Task<IReadOnlyList<CourseWithInstanceCount>> GetAllWithInstanceCountAsync(CancellationToken ct = default);
}
public sealed record CourseWithInstanceCount(Course Course, int InstanceCount);
