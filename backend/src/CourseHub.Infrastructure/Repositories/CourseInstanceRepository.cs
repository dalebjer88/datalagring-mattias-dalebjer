using CourseHub.Application.CourseInstances;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CourseHub.Infrastructure.Persistence.ReadModels;


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
            .Include(x => x.CourseInstanceTeachers)
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.Id)
            .ToListAsync(ct);
    }

    public override Task<CourseInstance?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return Set
            .AsNoTracking()
            .Include(x => x.CourseInstanceTeachers)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public override Task<CourseInstance?> GetForUpdateAsync(int id, CancellationToken ct = default)
    {
        return Set
            .Include(x => x.CourseInstanceTeachers)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> CourseExistsAsync(int courseId, CancellationToken ct = default)
    {
        return Db.Courses.AnyAsync(x => x.Id == courseId, ct);
    }

    public Task<bool> LocationExistsAsync(int locationId, CancellationToken ct = default)
    {
        return Db.Locations.AnyAsync(x => x.Id == locationId, ct);
    }

    public async Task<bool> TeachersExistAsync(IEnumerable<int> teacherIds, CancellationToken ct = default)
    {
        var ids = teacherIds
            .Where(x => x > 0)
            .Distinct()
            .ToArray();

        if (ids.Length == 0) return false;

        var count = await Db.Teachers.CountAsync(x => ids.Contains(x.Id), ct);
        return count == ids.Length;
    }
    public async Task<IReadOnlyList<CourseInstanceWithEnrollmentCountDto>> GetAllWithEnrollmentCountRawSqlAsync(CancellationToken ct = default)
    {
        const string sql = """
        SELECT
            ci.Id,
            ci.StartDate,
            ci.EndDate,
            ci.Capacity,
            ci.CourseId,
            ci.LocationId,
            COUNT(e.Id) AS EnrollmentCount
        FROM CourseInstances AS ci
        LEFT JOIN Enrollments AS e ON e.CourseInstanceId = ci.Id
        GROUP BY
            ci.Id,
            ci.StartDate,
            ci.EndDate,
            ci.Capacity,
            ci.CourseId,
            ci.LocationId
        ORDER BY
            ci.StartDate,
            ci.Id
        """;

        var rows = await Db.Set<CourseInstanceWithEnrollmentCountRow>()
            .FromSqlRaw(sql)
            .AsNoTracking()
            .ToListAsync(ct);

        return rows
            .Select(x => new CourseInstanceWithEnrollmentCountDto(
                x.Id,
                x.StartDate,
                x.EndDate,
                x.Capacity,
                x.CourseId,
                x.LocationId,
                x.EnrollmentCount
            ))
            .ToList();
    }

}
