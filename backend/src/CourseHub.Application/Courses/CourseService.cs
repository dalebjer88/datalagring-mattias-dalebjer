using CourseHub.Domain.Entities;

namespace CourseHub.Application.Courses;

public sealed class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo)
    {
        _repo = repo;
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct = default)
    {
        var courseCode = request.CourseCode.Trim();
        var title = request.Title.Trim();
        var description = request.Description.Trim();

        if (await _repo.CourseCodeExistsAsync(courseCode, ct))
            throw new InvalidOperationException("Course code already exists.");

        var course = new Course
        {
            CourseCode = courseCode,
            Title = title,
            Description = description
        };

        var created = await _repo.AddAsync(course, ct);

        return new CourseDto(created.Id, created.CourseCode, created.Title, created.Description);
    }

    public async Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);

        return items
            .Select(x => new CourseDto(x.Id, x.CourseCode, x.Title, x.Description))
            .ToList();
    }

    public async Task<CourseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var course = await _repo.GetByIdAsync(id, ct);
        if (course is null) return null;

        return new CourseDto(course.Id, course.CourseCode, course.Title, course.Description);
    }

    public async Task<CourseDto?> UpdateAsync(int id, UpdateCourseRequest request, CancellationToken ct = default)
    {
        var course = await _repo.GetForUpdateAsync(id, ct);
        if (course is null) return null;

        var courseCode = request.CourseCode.Trim();
        var title = request.Title.Trim();
        var description = request.Description.Trim();

        if (!string.Equals(course.CourseCode, courseCode, StringComparison.Ordinal))
        {
            var existing = await _repo.GetByCourseCodeAsync(courseCode, ct);
            if (existing is not null && existing.Id != id)
                throw new InvalidOperationException("Course code already exists.");
        }

        course.CourseCode = courseCode;
        course.Title = title;
        course.Description = description;

        await _repo.SaveChangesAsync(ct);

        return new CourseDto(course.Id, course.CourseCode, course.Title, course.Description);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var course = await _repo.GetForUpdateAsync(id, ct);
        if (course is null) return false;

        await _repo.RemoveAsync(course, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }
}
