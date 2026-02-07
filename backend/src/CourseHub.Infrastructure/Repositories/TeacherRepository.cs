using CourseHub.Application.Teachers;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public sealed class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(CourseHubDbContext db) : base(db)
    {
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return Db.Teachers.AnyAsync(x => x.Email == email, ct);
    }

    public Task<Teacher?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return Db.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public Task<bool> IsUsedByCourseInstancesAsync(int teacherId, CancellationToken ct = default)
    {
        return Db.CourseInstanceTeachers.AnyAsync(x => x.TeacherId == teacherId, ct);
    }

    public override async Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken ct = default)
    {
        return await Db.Teachers
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(ct);
    }
}
