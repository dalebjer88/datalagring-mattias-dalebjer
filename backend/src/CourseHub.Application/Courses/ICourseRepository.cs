using CourseHub.Domain.Entities;

namespace CourseHub.Application.Courses;

public interface ICourseRepository
{
    Task<bool> CourseCodeExistsAsync(string courseCode, CancellationToken ct = default);
    Task<Course> AddAsync(Course course, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);

    Task<Course?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Course?> GetByCourseCodeAsync(string courseCode, CancellationToken ct = default);

    Task<Course?> GetForUpdateAsync(int id, CancellationToken ct = default);
    Task RemoveAsync(Course course, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
