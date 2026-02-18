using CourseHub.Application.Common.Exceptions;
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
            throw new ValidationException("Course does not exist.");

        if (!await _repo.LocationExistsAsync(request.LocationId, ct))
            throw new ValidationException("Location does not exist.");

        var teacherIds = (request.TeacherIds ?? Array.Empty<int>())
            .Where(x => x > 0)
            .Distinct()
            .ToArray();

        if (teacherIds.Length == 0)
            throw new ValidationException("At least one teacher is required.");

        if (!await _repo.TeachersExistAsync(teacherIds, ct))
            throw new ValidationException("One or more teachers do not exist.");

        var entity = new CourseInstance
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Capacity = request.Capacity,
            CourseId = request.CourseId,
            LocationId = request.LocationId
        };

        foreach (var teacherId in teacherIds)
        {
            entity.CourseInstanceTeachers.Add(new CourseInstanceTeacher
            {
                TeacherId = teacherId
            });
        }

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        var dtoTeacherIds = entity.CourseInstanceTeachers.Select(x => x.TeacherId).ToArray();
        return new CourseInstanceDto(entity.Id, entity.StartDate, entity.EndDate, entity.Capacity, entity.CourseId, entity.LocationId, dtoTeacherIds);
    }

    public async Task<IReadOnlyList<CourseInstanceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);

        return items
            .Select(x => new CourseInstanceDto(
                x.Id,
                x.StartDate,
                x.EndDate,
                x.Capacity,
                x.CourseId,
                x.LocationId,
                x.CourseInstanceTeachers.Select(t => t.TeacherId).ToArray()
            ))
            .ToList();
    }

    public async Task<CourseInstanceDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) throw new NotFoundException("Course instance not found.");

        var teacherIds = entity.CourseInstanceTeachers.Select(t => t.TeacherId).ToArray();
        return new CourseInstanceDto(entity.Id, entity.StartDate, entity.EndDate, entity.Capacity, entity.CourseId, entity.LocationId, teacherIds);
    }

    public async Task<CourseInstanceDto> UpdateAsync(int id, UpdateCourseInstanceRequest request, CancellationToken ct = default)
    {
        Validate(request.StartDate, request.EndDate, request.Capacity, request.CourseId, request.LocationId);

        var entity = await _repo.GetForUpdateAsync(id, ct);
        if (entity is null) throw new NotFoundException("Course instance not found.");

        if (!await _repo.CourseExistsAsync(request.CourseId, ct))
            throw new ValidationException("Course does not exist.");

        if (!await _repo.LocationExistsAsync(request.LocationId, ct))
            throw new ValidationException("Location does not exist.");

        var teacherIds = (request.TeacherIds ?? Array.Empty<int>())
            .Where(x => x > 0)
            .Distinct()
            .ToArray();

        if (teacherIds.Length == 0)
            throw new ValidationException("At least one teacher is required.");

        if (!await _repo.TeachersExistAsync(teacherIds, ct))
            throw new ValidationException("One or more teachers do not exist.");

        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.Capacity = request.Capacity;
        entity.CourseId = request.CourseId;
        entity.LocationId = request.LocationId;

        entity.CourseInstanceTeachers.Clear();

        foreach (var teacherId in teacherIds)
        {
            entity.CourseInstanceTeachers.Add(new CourseInstanceTeacher
            {
                CourseInstanceId = entity.Id,
                TeacherId = teacherId
            });
        }

        await _repo.SaveChangesAsync(ct);

        var dtoTeacherIds = entity.CourseInstanceTeachers.Select(x => x.TeacherId).ToArray();
        return new CourseInstanceDto(entity.Id, entity.StartDate, entity.EndDate, entity.Capacity, entity.CourseId, entity.LocationId, dtoTeacherIds);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetForUpdateAsync(id, ct);
        if (entity is null) throw new NotFoundException("Course instance not found.");

        await _repo.RemoveAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<CourseInstanceWithEnrollmentCountDto>> GetAllWithEnrollmentCountAsync(CancellationToken ct = default)
    {
        return _repo.GetAllWithEnrollmentCountRawSqlAsync(ct);
    }

    private static void Validate(DateOnly startDate, DateOnly endDate, int capacity, int courseId, int locationId)
    {
        if (endDate < startDate) throw new ValidationException("End date must be on or after start date.");
        if (capacity <= 0) throw new ValidationException("Capacity must be greater than 0.");
        if (courseId <= 0) throw new ValidationException("CourseId is required.");
        if (locationId <= 0) throw new ValidationException("LocationId is required.");
    }
}
