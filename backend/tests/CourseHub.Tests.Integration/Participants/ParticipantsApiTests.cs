using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.Participants;
using CourseHub.Tests.Integration.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Tests.Integration.Participants;

public sealed class ParticipantsApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ApiTestClient _api;

    public ParticipantsApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _api = new ApiTestClient(_client);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsParticipant()
    {
        await _factory.ResetDbAsync();

        var created = await _api.CreateParticipantAsync(email: "a@test.com", firstName: "A", lastName: "B", phoneNumber: "0701");

        var res = await _client.GetAsync($"/api/participants/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ParticipantDto>();
        Assert.NotNull(body);
        Assert.Equal("a@test.com", body!.Email);
    }

    [Fact]
    public async Task Create_DuplicateEmail_ReturnsConflictProblemDetails()
    {
        await _factory.ResetDbAsync();

        _ = await _api.CreateParticipantAsync(email: "dup@test.com");

        var res = await _client.PostAsJsonAsync("/api/participants",
            new CreateParticipantRequest("dup@test.com", "X", "Y", "0702"));

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(409, body!.Status);
    }
}
