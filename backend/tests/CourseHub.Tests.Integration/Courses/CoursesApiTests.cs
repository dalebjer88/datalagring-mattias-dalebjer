using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.Courses;
using CourseHub.Tests.Integration.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Tests.Integration.Courses;

public sealed class CoursesApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ApiTestClient _api;

    public CoursesApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _api = new ApiTestClient(_client);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFoundProblemDetails()
    {
        await _factory.ResetDbAsync();

        var res = await _client.GetAsync("/api/courses/999999");

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(404, body!.Status);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCourse()
    {
        await _factory.ResetDbAsync();

        var created = await _api.CreateCourseAsync(code: "NET-201", title: "EF Core", description: "Data");

        var res = await _client.GetAsync($"/api/courses/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<CourseDto>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body!.Id);
        Assert.Equal("NET-201", body.CourseCode);
    }

    [Fact]
    public async Task Create_DuplicateCourseCode_ReturnsConflictProblemDetails()
    {
        await _factory.ResetDbAsync();

        _ = await _api.CreateCourseAsync(code: "NET-301", title: "API", description: "Minimal");

        var res = await _client.PostAsJsonAsync("/api/courses", new CreateCourseRequest("NET-301", "Other", "Other"));

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(409, body!.Status);
    }
}
