using CourseHub.Application.CourseInstances;
using CourseHub.Application.Courses;
using CourseHub.Application.Participants;
using CourseHub.Infrastructure;
using CourseHub.Application.Locations;
using CourseHub.Application.Enrollments;
using CourseHub.Application.Teachers;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<ICourseInstanceService, CourseInstanceService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();

app.UseCors("frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "CourseHub API is running");
app.MapGet("/health", () => "OK");

var api = app.MapGroup("/api");


var courses = api.MapGroup("/courses");

courses.MapGet("/", async (ICourseService service, CancellationToken ct) =>
{
    var courses = await service.GetAllAsync(ct);
    return Results.Ok(courses);
});

courses.MapPost("/", async (CreateCourseRequest request, ICourseService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(request, ct);
        return Results.Created($"/api/courses/{created.Id}", created);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Course code already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

courses.MapGet("/{id:int}", async (int id, ICourseService service, CancellationToken ct) =>
{
    var course = await service.GetByIdAsync(id, ct);
    return course is null ? Results.NotFound() : Results.Ok(course);
});

courses.MapPut("/{id:int}", async (int id, UpdateCourseRequest request, ICourseService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Course code already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

courses.MapDelete("/{id:int}", async (int id, ICourseService service, CancellationToken ct) =>
{
    try
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
    catch (InvalidOperationException ex) when (ex.Message == "Course is used by course instances. Remove instances first.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

var participants = api.MapGroup("/participants");

participants.MapGet("/", async (IParticipantService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

participants.MapPost("/", async (CreateParticipantRequest request, IParticipantService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(request, ct);
        return Results.Created($"/api/participants/{created.Id}", created);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Email already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

participants.MapGet("/{id:int}", async (int id, IParticipantService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

participants.MapPut("/{id:int}", async (int id, UpdateParticipantRequest request, IParticipantService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Email already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

participants.MapDelete("/{id:int}", async (int id, IParticipantService service, CancellationToken ct) =>
{
    var deleted = await service.DeleteAsync(id, ct);
    return deleted ? Results.NoContent() : Results.NotFound();
});


var courseInstances = api.MapGroup("/course-instances");

courseInstances.MapGet("/", async (ICourseInstanceService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

courseInstances.MapPost("/", async (CreateCourseInstanceRequest request, ICourseInstanceService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(request, ct);
        return Results.Created($"/api/course-instances/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

courseInstances.MapGet("/{id:int}", async (int id, ICourseInstanceService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

courseInstances.MapPut("/{id:int}", async (int id, UpdateCourseInstanceRequest request, ICourseInstanceService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

courseInstances.MapDelete("/{id:int}", async (int id, ICourseInstanceService service, CancellationToken ct) =>
{
    var deleted = await service.DeleteAsync(id, ct);
    return deleted ? Results.NoContent() : Results.NotFound();
});


var locations = api.MapGroup("/locations");

locations.MapGet("/", async (ILocationService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

locations.MapPost("/", async (CreateLocationRequest request, ILocationService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(request, ct);
        return Results.Created($"/api/locations/{created.Id}", created);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Location name already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

locations.MapGet("/{id:int}", async (int id, ILocationService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

locations.MapPut("/{id:int}", async (int id, UpdateLocationRequest request, ILocationService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Location name already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

locations.MapDelete("/{id:int}", async (int id, ILocationService service, CancellationToken ct) =>
{
    try
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
    catch (InvalidOperationException ex) when (ex.Message == "Location is used by course instances. Remove instances first.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

var enrollments = api.MapGroup("/enrollments");

enrollments.MapGet("/", async (IEnrollmentService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

enrollments.MapPost("/", async (CreateEnrollmentRequest request, IEnrollmentService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(request, ct);
        return Results.Created($"/api/enrollments/{created.Id}", created);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Enrollment already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

enrollments.MapGet("/{id:int}", async (int id, IEnrollmentService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

enrollments.MapPut("/{id:int}", async (int id, UpdateEnrollmentRequest request, IEnrollmentService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

enrollments.MapDelete("/{id:int}", async (int id, IEnrollmentService service, CancellationToken ct) =>
{
    var deleted = await service.DeleteAsync(id, ct);
    return deleted ? Results.NoContent() : Results.NotFound();
});

var teachers = api.MapGroup("/teachers");

teachers.MapGet("/", async (ITeacherService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

teachers.MapPost("/", async (CreateTeacherRequest request, ITeacherService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(request, ct);
        return Results.Created($"/api/teachers/{created.Id}", created);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Email already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

teachers.MapGet("/{id:int}", async (int id, ITeacherService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

teachers.MapPut("/{id:int}", async (int id, UpdateTeacherRequest request, ITeacherService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Email already exists.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

teachers.MapDelete("/{id:int}", async (int id, ITeacherService service, CancellationToken ct) =>
{
    try
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
    catch (InvalidOperationException ex) when (ex.Message == "Teacher is used by course instances. Remove links first.")
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

app.Run();
