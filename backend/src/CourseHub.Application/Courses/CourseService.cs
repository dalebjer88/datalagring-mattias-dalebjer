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
}
