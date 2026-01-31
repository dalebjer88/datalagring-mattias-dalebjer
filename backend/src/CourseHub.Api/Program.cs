using CourseHub.Application.Courses;
using CourseHub.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICourseService, CourseService>();

var app = builder.Build();

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

app.Run();
