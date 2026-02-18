using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.Teachers;
using CourseHub.Tests.Integration.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Tests.Integration.Teachers;

public sealed class TeachersApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ApiTestClient _api;

    public TeachersApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _api = new ApiTestClient(_client);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsTeacher()
    {
        await _factory.ResetDbAsync();

        var created = await _api.CreateTeacherAsync(email: "teach@test.com");

        var res = await _client.GetAsync($"/api/teachers/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<TeacherDto>();
        Assert.NotNull(body);
        Assert.Equal("teach@test.com", body!.Email);
    }

    [Fact]
    public async Task Create_DuplicateEmail_ReturnsConflictProblemDetails()
    {
        await _factory.ResetDbAsync();

        _ = await _api.CreateTeacherAsync(email: "dupteacher@test.com");

        var res = await _client.PostAsJsonAsync("/api/teachers",
            new CreateTeacherRequest("dupteacher@test.com", "A", "B", "SQL"));

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(409, body!.Status);
    }
}
