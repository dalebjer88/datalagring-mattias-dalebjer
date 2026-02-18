using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.CourseInstances;
using CourseHub.Tests.Integration.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Tests.Integration.CourseInstances;

public sealed class CourseInstancesApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ApiTestClient _api;

    public CourseInstancesApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _api = new ApiTestClient(_client);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCourseInstance()
    {
        await _factory.ResetDbAsync();

        var course = await _api.CreateCourseAsync(code: "CI-101");
        var location = await _api.CreateLocationAsync("Göteborg");
        var teacher = await _api.CreateTeacherAsync(email: "ci.teacher@test.com");

        var created = await _api.CreateCourseInstanceAsync(course.Id, location.Id, new[] { teacher.Id });

        var res = await _client.GetAsync($"/api/course-instances/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<CourseInstanceDto>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body!.Id);
        Assert.Contains(teacher.Id, body.TeacherIds);
    }

    [Fact]
    public async Task Create_WhenNoTeachers_ReturnsBadRequestValidationProblemDetails()
    {
        await _factory.ResetDbAsync();

        var course = await _api.CreateCourseAsync(code: "CI-201");
        var location = await _api.CreateLocationAsync("Malmö");

        var req = new CreateCourseInstanceRequest(
            new DateOnly(2030, 1, 1),
            new DateOnly(2030, 1, 2),
            10,
            course.Id,
            location.Id,
            Array.Empty<int>());

        var res = await _client.PostAsJsonAsync("/api/course-instances", req);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(400, body!.Status);
    }

    [Fact]
    public async Task GetWithEnrollmentCount_ReturnsCorrectCounts()
    {
        await _factory.ResetDbAsync();

        var course = await _api.CreateCourseAsync(code: "SQL-COUNT");
        var location = await _api.CreateLocationAsync("Örebro");
        var teacher = await _api.CreateTeacherAsync(email: "count.teacher@test.com");
        var p1 = await _api.CreateParticipantAsync(email: "p1@test.com");
        var p2 = await _api.CreateParticipantAsync(email: "p2@test.com");

        var ci = await _api.CreateCourseInstanceAsync(course.Id, location.Id, new[] { teacher.Id });

        _ = await _api.CreateEnrollmentAsync(p1.Id, ci.Id, "Active");
        _ = await _api.CreateEnrollmentAsync(p2.Id, ci.Id, "Active");

        var res = await _client.GetAsync("/api/course-instances/with-enrollment-count");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<List<CourseInstanceWithEnrollmentCountDto>>();
        Assert.NotNull(body);

        var row = body!.Single(x => x.Id == ci.Id);
        Assert.Equal(2, row.EnrollmentCount);
    }
}
