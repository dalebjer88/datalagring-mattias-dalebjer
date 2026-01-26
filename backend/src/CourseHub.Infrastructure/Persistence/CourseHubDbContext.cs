using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence;

public sealed class CourseHubDbContext : DbContext
{
    public CourseHubDbContext(DbContextOptions<CourseHubDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseInstance> CourseInstances => Set<CourseInstance>();
    public DbSet<CourseInstanceTeacher> CourseInstanceTeachers => Set<CourseInstanceTeacher>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Teacher> Teachers => Set<Teacher>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseHubDbContext).Assembly);
    }
}
