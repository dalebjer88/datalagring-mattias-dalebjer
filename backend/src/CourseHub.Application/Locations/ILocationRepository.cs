using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Locations;

public interface ILocationRepository : IBaseRepository<Location>
{
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<Location?> GetByNameAsync(string name, CancellationToken ct = default);
}
