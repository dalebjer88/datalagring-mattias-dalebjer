using CourseHub.Application.CourseInstances;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public sealed class CourseInstanceRepository : BaseRepository<CourseInstance>, ICourseInstanceRepository
{
    public CourseInstanceRepository(CourseHubDbContext db) : base(db)
    {
    }

    public override async Task<IReadOnlyList<CourseInstance>> GetAllAsync(CancellationToken ct = default)
    {
        return await Set
            .AsNoTracking()
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.Id)
            .ToListAsync(ct);
    }

    public Task<bool> CourseExistsAsync(int courseId, CancellationToken ct = default)
    {
        return Db.Courses.AnyAsync(x => x.Id == courseId, ct);
    }

    public Task<bool> LocationExistsAsync(int locationId, CancellationToken ct = default)
    {
        return Db.Locations.AnyAsync(x => x.Id == locationId, ct);
    }
}
