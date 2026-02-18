using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.CourseInstances;
using CourseHub.Application.Enrollments;
using CourseHub.Tests.Integration.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Tests.Integration.Registrations;

public sealed class RegistrationsApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ApiTestClient _api;

    public RegistrationsApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _api = new ApiTestClient(_client);
    }

    [Fact]
    public async Task CreateCourseInstanceWithEnrollments_CreatesInstanceAndEnrollments()
    {
        await _factory.ResetDbAsync();

        var course = await _api.CreateCourseAsync(code: "REG-101");
        var location = await _api.CreateLocationAsync("Helsingborg");
        var teacher = await _api.CreateTeacherAsync(email: "reg.teacher@test.com");
        var p1 = await _api.CreateParticipantAsync(email: "reg.p1@test.com");
        var p2 = await _api.CreateParticipantAsync(email: "reg.p2@test.com");

        var result = await _api.RegisterCourseInstanceWithEnrollmentsAsync(
            course.Id,
            location.Id,
            new[] { teacher.Id },
            new[] { p1.Id, p2.Id },
            status: "Active");

        Assert.True(result.CourseInstanceId > 0);
        Assert.Equal(2, result.EnrollmentCount);

        var ciRes = await _client.GetAsync($"/api/course-instances/{result.CourseInstanceId}");
        Assert.Equal(HttpStatusCode.OK, ciRes.StatusCode);

        var enrRes = await _client.GetAsync("/api/enrollments");
        Assert.Equal(HttpStatusCode.OK, enrRes.StatusCode);

        var enrollments = await enrRes.Content.ReadFromJsonAsync<List<EnrollmentDto>>();
        Assert.NotNull(enrollments);
        Assert.Equal(2, enrollments!.Count);
    }

    [Fact]
    public async Task CreateCourseInstanceWithEnrollments_WhenParticipantMissing_ReturnsBadRequestAndNoWrites()
    {
        await _factory.ResetDbAsync();

        var course = await _api.CreateCourseAsync(code: "REG-201");
        var location = await _api.CreateLocationAsync("Lund");
        var teacher = await _api.CreateTeacherAsync(email: "reg2.teacher@test.com");

        var res = await _client.PostAsJsonAsync("/api/registrations/course-instance-with-enrollments",
            new CourseHub.Application.Registrations.CreateCourseInstanceWithEnrollmentsRequest(
                new DateOnly(2030, 2, 1),
                new DateOnly(2030, 2, 2),
                10,
                course.Id,
                location.Id,
                new[] { teacher.Id },
                new[] { 999999 },
                "Active"));

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(400, body!.Status);

        var ciRes = await _client.GetAsync("/api/course-instances");
        var instances = await ciRes.Content.ReadFromJsonAsync<List<CourseInstanceDto>>();
        Assert.NotNull(instances);
        Assert.Empty(instances!);

        var enrRes = await _client.GetAsync("/api/enrollments");
        var enrollments = await enrRes.Content.ReadFromJsonAsync<List<EnrollmentDto>>();
        Assert.NotNull(enrollments);
        Assert.Empty(enrollments!);
    }
}
    