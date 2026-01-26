using CourseHub.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "CourseHub API is running");
app.MapGet("/health", () => "OK");

app.Run();
