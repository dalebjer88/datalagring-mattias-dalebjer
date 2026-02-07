using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Teachers;

public interface ITeacherRepository : IBaseRepository<Teacher>
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<Teacher?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> IsUsedByCourseInstancesAsync(int teacherId, CancellationToken ct = default);
}
