using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CourseHub.Application.Courses;
using CourseHub.Infrastructure.Repositories;
using CourseHub.Application.Participants;
using CourseHub.Application.CourseInstances;
using CourseHub.Application.Locations;
using CourseHub.Application.Enrollments;
using CourseHub.Application.Teachers;





namespace CourseHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CourseHubDb");

        services.AddDbContext<CourseHubDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<ICourseInstanceRepository, CourseInstanceRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();



        return services;
    }
}
