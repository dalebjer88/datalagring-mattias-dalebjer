using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Courses;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<bool> CourseCodeExistsAsync(string courseCode, CancellationToken ct = default);
    Task<Course?> GetByCourseCodeAsync(string courseCode, CancellationToken ct = default);
}
