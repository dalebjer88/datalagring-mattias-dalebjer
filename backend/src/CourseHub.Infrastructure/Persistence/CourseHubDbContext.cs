using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        modelBuilder.Entity<CourseInstanceWithEnrollmentCountRow>(builder =>
        {
            builder.HasNoKey();
            builder.ToView(null);
        });

        if (Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true)
            ApplySqliteDateOnlyConversions(modelBuilder);
    }

    private static void ApplySqliteDateOnlyConversions(ModelBuilder modelBuilder)
    {
        var converter = new ValueConverter<DateOnly, string>(
            v => v.ToString("yyyy-MM-dd"),
            v => DateOnly.Parse(v));

        var comparer = new ValueComparer<DateOnly>(
            (l, r) => l.DayNumber == r.DayNumber,
            v => v.GetHashCode(),
            v => DateOnly.FromDayNumber(v.DayNumber));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(DateOnly))
                    continue;

                property.SetValueConverter(converter);
                property.SetValueComparer(comparer);
            }
        }
    }
}
