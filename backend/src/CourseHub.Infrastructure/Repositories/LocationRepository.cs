using CourseHub.Application.Locations;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public sealed class LocationRepository : BaseRepository<Location>, ILocationRepository
{
    public LocationRepository(CourseHubDbContext db) : base(db)
    {
    }

    public override async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default)
    {
        return await Set
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
    {
        return Db.Locations.AnyAsync(x => x.Name == name, ct);
    }

    public Task<Location?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return Db.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, ct);
    }
    public Task<bool> IsUsedByCourseInstancesAsync(int locationId, CancellationToken ct = default)
    {
        return Db.CourseInstances.AnyAsync(x => x.LocationId == locationId, ct);
    }
    public async Task<IReadOnlyList<LocationWithInstanceCount>> GetAllWithInstanceCountAsync(CancellationToken ct = default)
    {
        var items = await Db.Locations
            .AsNoTracking()
            .Select(l => new LocationWithInstanceCount(
                l,
                Db.CourseInstances.Count(ci => ci.LocationId == l.Id)
            ))
            .ToListAsync(ct);

        return items;
    }

}
