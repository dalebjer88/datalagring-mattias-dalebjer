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
    public Task<bool> IsUsedByCourseInstancesAsync(int courseId, CancellationToken ct = default)
    {
        return Db.CourseInstances.AnyAsync(x => x.CourseId == courseId, ct);
    }
    public async Task<IReadOnlyList<CourseWithInstanceCount>> GetAllWithInstanceCountAsync(CancellationToken ct = default)
    {
        var items = await Db.Courses
            .Select(c => new CourseWithInstanceCount(
                c,
                Db.CourseInstances.Count(ci => ci.CourseId == c.Id)
            ))
            .ToListAsync(ct);

        return items;
    }

}
