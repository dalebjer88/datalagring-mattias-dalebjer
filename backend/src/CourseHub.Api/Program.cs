var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "CourseHub API is running");
app.MapGet("/health", () => "OK");

app.Run();
