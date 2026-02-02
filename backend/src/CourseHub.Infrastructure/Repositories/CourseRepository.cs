using CourseHub.Application.Courses;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public sealed class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(CourseHubDbContext db) : base(db)
    {
    }

    public Task<bool> CourseCodeExistsAsync(string courseCode, CancellationToken ct = default)
    {
        return Db.Courses.AnyAsync(x => x.CourseCode == courseCode, ct);
    }

    public override async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default)
    {
        return await Db.Courses
            .AsNoTracking()
            .OrderBy(x => x.CourseCode)
            .ToListAsync(ct);
    }

    public Task<Course?> GetByCourseCodeAsync(string courseCode, CancellationToken ct = default)
    {
        return Db.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CourseCode == courseCode, ct);
    }
}
