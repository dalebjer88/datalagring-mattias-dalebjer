using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.CourseInstances;
using CourseHub.Application.Courses;
using CourseHub.Application.Enrollments;
using CourseHub.Application.Locations;
using CourseHub.Application.Participants;
using CourseHub.Application.Registrations;
using CourseHub.Application.Teachers;
using CourseHub.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<ICourseInstanceService, CourseInstanceService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

#region Exception Handling
app.UseExceptionHandler(handlerApp =>
{
    handlerApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ex is null)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred."
            });
            return;
        }

        if (ex is ValidationException vex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(vex.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = vex.Message
            });

            return;
        }

        if (ex is NotFoundException nfex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = nfex.Message
            });
            return;
        }

        if (ex is ConflictException cfex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = cfex.Message
            });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred."
        });
    });
});
#endregion

app.UseCors("frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "CourseHub API is running");
app.MapGet("/health", () => "OK");

var api = app.MapGroup("/api");

#region Courses
var courses = api.MapGroup("/courses").WithTags("Courses");

courses.MapGet("/", async (ICourseService service, IMemoryCache cache, CancellationToken ct) =>
{
    const string key = "courses:all";

    if (cache.TryGetValue(key, out IReadOnlyList<CourseDto>? cached) && cached is not null)
        return Results.Ok(cached);

    var items = await service.GetAllAsync(ct);

    cache.Set(key, items, TimeSpan.FromSeconds(60));
    return Results.Ok(items);
});

courses.MapPost("/", async (CreateCourseRequest request, ICourseService service, IMemoryCache cache, CancellationToken ct) =>
{
    var created = await service.CreateAsync(request, ct);
    cache.Remove("courses:all");
    return Results.Created($"/api/courses/{created.Id}", created);
});

courses.MapGet("/{id:int}", async (int id, ICourseService service, CancellationToken ct) =>
{
    var course = await service.GetByIdAsync(id, ct);
    return Results.Ok(course);
});

courses.MapPut("/{id:int}", async (int id, UpdateCourseRequest request, ICourseService service, IMemoryCache cache, CancellationToken ct) =>
{
    var updated = await service.UpdateAsync(id, request, ct);
    cache.Remove("courses:all");
    return Results.Ok(updated);
});

courses.MapDelete("/{id:int}", async (int id, ICourseService service, IMemoryCache cache, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    cache.Remove("courses:all");
    return Results.NoContent();
});

#endregion

#region Participants
var participants = api.MapGroup("/participants").WithTags("Participants");

participants.MapGet("/", async (IParticipantService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

participants.MapPost("/", async (CreateParticipantRequest request, IParticipantService service, CancellationToken ct) =>
{
    var created = await service.CreateAsync(request, ct);
    return Results.Created($"/api/participants/{created.Id}", created);
});

participants.MapGet("/{id:int}", async (int id, IParticipantService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return Results.Ok(item);
});

participants.MapPut("/{id:int}", async (int id, UpdateParticipantRequest request, IParticipantService service, CancellationToken ct) =>
{
    var updated = await service.UpdateAsync(id, request, ct);
    return Results.Ok(updated);
});

participants.MapDelete("/{id:int}", async (int id, IParticipantService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
});

#endregion

#region CourseInstances
var courseInstances = api.MapGroup("/course-instances").WithTags("Course-Instances");

courseInstances.MapGet("/", async (ICourseInstanceService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

courseInstances.MapGet("/with-enrollment-count", async (ICourseInstanceService service, CancellationToken ct) =>
{
    var items = await service.GetAllWithEnrollmentCountAsync(ct);
    return Results.Ok(items);
});

courseInstances.MapPost("/", async (CreateCourseInstanceRequest request, ICourseInstanceService service, CancellationToken ct) =>
{
    var created = await service.CreateAsync(request, ct);
    return Results.Created($"/api/course-instances/{created.Id}", created);
});

courseInstances.MapGet("/{id:int}", async (int id, ICourseInstanceService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return Results.Ok(item);
});

courseInstances.MapPut("/{id:int}", async (int id, UpdateCourseInstanceRequest request, ICourseInstanceService service, CancellationToken ct) =>
{
    var updated = await service.UpdateAsync(id, request, ct);
    return Results.Ok(updated);
});

courseInstances.MapDelete("/{id:int}", async (int id, ICourseInstanceService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
});

#endregion

#region Locations
var locations = api.MapGroup("/locations").WithTags("Locations");

locations.MapGet("/", async (ILocationService service, IMemoryCache cache, CancellationToken ct) =>
{
    const string key = "locations:all";

    if (cache.TryGetValue(key, out IReadOnlyList<LocationDto>? cached) && cached is not null)
        return Results.Ok(cached);

    var items = await service.GetAllAsync(ct);

    cache.Set(key, items, TimeSpan.FromSeconds(60));
    return Results.Ok(items);
});

locations.MapPost("/", async (CreateLocationRequest request, ILocationService service, IMemoryCache cache, CancellationToken ct) =>
{
    var created = await service.CreateAsync(request, ct);
    cache.Remove("locations:all");
    return Results.Created($"/api/locations/{created.Id}", created);
});

locations.MapGet("/{id:int}", async (int id, ILocationService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return Results.Ok(item);
});

locations.MapPut("/{id:int}", async (int id, UpdateLocationRequest request, ILocationService service, IMemoryCache cache, CancellationToken ct) =>
{
    var updated = await service.UpdateAsync(id, request, ct);
    cache.Remove("locations:all");
    return Results.Ok(updated);
});

locations.MapDelete("/{id:int}", async (int id, ILocationService service, IMemoryCache cache, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    cache.Remove("locations:all");
    return Results.NoContent();
});

#endregion

#region Enrollments
var enrollments = api.MapGroup("/enrollments").WithTags("Enrollments");

enrollments.MapGet("/", async (IEnrollmentService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

enrollments.MapPost("/", async (CreateEnrollmentRequest request, IEnrollmentService service, CancellationToken ct) =>
{
    var created = await service.CreateAsync(request, ct);
    return Results.Created($"/api/enrollments/{created.Id}", created);
});

enrollments.MapGet("/{id:int}", async (int id, IEnrollmentService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return Results.Ok(item);
});

enrollments.MapPut("/{id:int}", async (int id, UpdateEnrollmentRequest request, IEnrollmentService service, CancellationToken ct) =>
{
    var updated = await service.UpdateAsync(id, request, ct);
    return Results.Ok(updated);
});

enrollments.MapDelete("/{id:int}", async (int id, IEnrollmentService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
});

#endregion

#region Teachers
var teachers = api.MapGroup("/teachers").WithTags("Teachers");

teachers.MapGet("/", async (ITeacherService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

teachers.MapPost("/", async (CreateTeacherRequest request, ITeacherService service, CancellationToken ct) =>
{
    var created = await service.CreateAsync(request, ct);
    return Results.Created($"/api/teachers/{created.Id}", created);
});

teachers.MapGet("/{id:int}", async (int id, ITeacherService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return Results.Ok(item);
});

teachers.MapPut("/{id:int}", async (int id, UpdateTeacherRequest request, ITeacherService service, CancellationToken ct) =>
{
    var updated = await service.UpdateAsync(id, request, ct);
    return Results.Ok(updated);
});

teachers.MapDelete("/{id:int}", async (int id, ITeacherService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
});

#endregion

#region Registrations
var registrations = api.MapGroup("/registrations").WithTags("Registrations");

registrations.MapPost("/course-instance-with-enrollments",
    async (CreateCourseInstanceWithEnrollmentsRequest request, IRegistrationService service, CancellationToken ct) =>
    {
        var result = await service.CreateCourseInstanceWithEnrollmentsAsync(request, ct);
        return Results.Created($"/api/course-instances/{result.CourseInstanceId}", result);
    });
#endregion

app.Run();
