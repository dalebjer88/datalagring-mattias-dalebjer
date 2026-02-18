using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.Enrollments;
using CourseHub.Tests.Integration.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Tests.Integration.Enrollments;

public sealed class EnrollmentsApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ApiTestClient _api;

    public EnrollmentsApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _api = new ApiTestClient(_client);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsEnrollment()
    {
        await _factory.ResetDbAsync();

        var course = await _api.CreateCourseAsync(code: "ENR-101");
        var location = await _api.CreateLocationAsync("Västerås");
        var teacher = await _api.CreateTeacherAsync(email: "enr.teacher@test.com");
        var participant = await _api.CreateParticipantAsync(email: "enr.p@test.com");

        var ci = await _api.CreateCourseInstanceAsync(course.Id, location.Id, new[] { teacher.Id });

        var created = await _api.CreateEnrollmentAsync(participant.Id, ci.Id, "Active");

        var res = await _client.GetAsync($"/api/enrollments/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<EnrollmentDto>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body!.Id);
        Assert.Equal("Active", body.Status);
    }

    [Fact]
    public async Task Create_DuplicateParticipantAndInstance_ReturnsConflictProblemDetails()
    {
        await _factory.ResetDbAsync();

        var course = await _api.CreateCourseAsync(code: "ENR-201");
        var location = await _api.CreateLocationAsync("Linköping");
        var teacher = await _api.CreateTeacherAsync(email: "enr2.teacher@test.com");
        var participant = await _api.CreateParticipantAsync(email: "enr2.p@test.com");

        var ci = await _api.CreateCourseInstanceAsync(course.Id, location.Id, new[] { teacher.Id });

        _ = await _api.CreateEnrollmentAsync(participant.Id, ci.Id, "Active");

        var res = await _client.PostAsJsonAsync("/api/enrollments",
            new CreateEnrollmentRequest(participant.Id, ci.Id, "Active"));

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(409, body!.Status);
    }
}
