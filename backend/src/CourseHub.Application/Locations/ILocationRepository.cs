using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Locations;

public interface ILocationRepository : IBaseRepository<Location>
{
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<Location?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> IsUsedByCourseInstancesAsync(int locationId, CancellationToken ct = default);
    Task<IReadOnlyList<LocationWithInstanceCount>> GetAllWithInstanceCountAsync(CancellationToken ct = default);
}

public sealed record LocationWithInstanceCount(Location Location, int InstanceCount);
