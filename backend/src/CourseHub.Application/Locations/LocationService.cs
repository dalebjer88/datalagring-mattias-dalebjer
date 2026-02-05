using CourseHub.Domain.Entities;

namespace CourseHub.Application.Locations;

public sealed class LocationService : ILocationService
{
    private readonly ILocationRepository _repo;

    public LocationService(ILocationRepository repo)
    {
        _repo = repo;
    }

    public async Task<LocationDto> CreateAsync(CreateLocationRequest request, CancellationToken ct = default)
    {
        var name = request.Name.Trim();
        Validate(name);

        if (await _repo.NameExistsAsync(name, ct))
            throw new InvalidOperationException("Location name already exists.");

        var entity = new Location
        {
            Name = name
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return new LocationDto(entity.Id, entity.Name, 0);
    }

    public async Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllWithInstanceCountAsync(ct);

        return items
            .Select(x => new LocationDto(x.Location.Id, x.Location.Name, x.InstanceCount))
            .ToList();
    }


    public async Task<LocationDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return null;

        return new LocationDto(entity.Id, entity.Name, 0);
    }

    public async Task<LocationDto?> UpdateAsync(int id, UpdateLocationRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.GetForUpdateAsync(id, ct);
        if (entity is null) return null;

        var name = request.Name.Trim();
        Validate(name);

        if (!string.Equals(entity.Name, name, StringComparison.Ordinal))
        {
            var existing = await _repo.GetByNameAsync(name, ct);
            if (existing is not null && existing.Id != id)
                throw new InvalidOperationException("Location name already exists.");
        }

        entity.Name = name;

        await _repo.SaveChangesAsync(ct);

        return new LocationDto(entity.Id, entity.Name, 0);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetForUpdateAsync(id, ct);
        if (entity is null) return false;

        if (await _repo.IsUsedByCourseInstancesAsync(id, ct))
            throw new InvalidOperationException("Location is used by course instances. Remove instances first.");

        await _repo.RemoveAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }


    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Location name is required.");
        if (name.Length > 100) throw new InvalidOperationException("Location name is too long.");
    }
}
