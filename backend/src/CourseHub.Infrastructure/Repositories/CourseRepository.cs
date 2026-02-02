using CourseHub.Application.Courses;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public sealed class CourseRepository : ICourseRepository
{
    private readonly CourseHubDbContext _db;

    public CourseRepository(CourseHubDbContext db)
    {
        _db = db;
    }

    public Task<bool> CourseCodeExistsAsync(string courseCode, CancellationToken ct = default)
    {
        return _db.Courses.AnyAsync(x => x.CourseCode == courseCode, ct);
    }

    public async Task<Course> AddAsync(Course course, CancellationToken ct = default)
    {
        _db.Courses.Add(course);
        await _db.SaveChangesAsync(ct);
        return course;
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Courses
            .AsNoTracking()
            .OrderBy(x => x.CourseCode)
            .ToListAsync(ct);
    }

    public Task<Course?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _db.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<Course?> GetByCourseCodeAsync(string courseCode, CancellationToken ct = default)
    {
        return _db.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CourseCode == courseCode, ct);
    }

    public Task<Course?> GetForUpdateAsync(int id, CancellationToken ct = default)
    {
        return _db.Courses.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task RemoveAsync(Course course, CancellationToken ct = default)
    {
        _db.Courses.Remove(course);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
