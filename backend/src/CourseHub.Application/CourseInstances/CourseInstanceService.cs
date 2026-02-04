using CourseHub.Domain.Entities;

namespace CourseHub.Application.CourseInstances;

public sealed class CourseInstanceService : ICourseInstanceService
{
    private readonly ICourseInstanceRepository _repo;

    public CourseInstanceService(ICourseInstanceRepository repo)
    {
        _repo = repo;
    }

    public async Task<CourseInstanceDto> CreateAsync(CreateCourseInstanceRequest request, CancellationToken ct = default)
    {
        Validate(request.StartDate, request.EndDate, request.Capacity, request.CourseId, request.LocationId);

        if (!await _repo.CourseExistsAsync(request.CourseId, ct))
            throw new InvalidOperationException("Course does not exist.");

        if (!await _repo.LocationExistsAsync(request.LocationId, ct))
            throw new InvalidOperationException("Location does not exist.");

        var entity = new CourseInstance
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Capacity = request.Capacity,
            CourseId = request.CourseId,
            LocationId = request.LocationId
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return new CourseInstanceDto(entity.Id, entity.StartDate, entity.EndDate, entity.Capacity, entity.CourseId, entity.LocationId);
    }

    public async Task<IReadOnlyList<CourseInstanceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);

        return items
            .Select(x => new CourseInstanceDto(x.Id, x.StartDate, x.EndDate, x.Capacity, x.CourseId, x.LocationId))
            .ToList();
    }

    public async Task<CourseInstanceDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return null;

        return new CourseInstanceDto(entity.Id, entity.StartDate, entity.EndDate, entity.Capacity, entity.CourseId, entity.LocationId);
    }

    public async Task<CourseInstanceDto?> UpdateAsync(int id, UpdateCourseInstanceRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.GetForUpdateAsync(id, ct);
        if (entity is null) return null;

        Validate(request.StartDate, request.EndDate, request.Capacity, request.CourseId, request.LocationId);

        if (!await _repo.CourseExistsAsync(request.CourseId, ct))
            throw new InvalidOperationException("Course does not exist.");

        if (!await _repo.LocationExistsAsync(request.LocationId, ct))
            throw new InvalidOperationException("Location does not exist.");

        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.Capacity = request.Capacity;
        entity.CourseId = request.CourseId;
        entity.LocationId = request.LocationId;

        await _repo.SaveChangesAsync(ct);

        return new CourseInstanceDto(entity.Id, entity.StartDate, entity.EndDate, entity.Capacity, entity.CourseId, entity.LocationId);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetForUpdateAsync(id, ct);
        if (entity is null) return false;

        await _repo.RemoveAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }

    private static void Validate(DateOnly startDate, DateOnly endDate, int capacity, int courseId, int locationId)
    {
        if (endDate < startDate) throw new InvalidOperationException("End date must be on or after start date.");
        if (capacity <= 0) throw new InvalidOperationException("Capacity must be greater than 0.");
        if (courseId <= 0) throw new InvalidOperationException("CourseId is required.");
        if (locationId <= 0) throw new InvalidOperationException("LocationId is required.");
    }
}
