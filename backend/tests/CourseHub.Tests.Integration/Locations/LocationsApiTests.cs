using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.Locations;
using CourseHub.Tests.Integration.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.Tests.Integration.Locations;

public sealed class LocationsApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ApiTestClient _api;

    public LocationsApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _api = new ApiTestClient(_client);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsLocation()
    {
        await _factory.ResetDbAsync();

        var created = await _api.CreateLocationAsync("Stockholm");

        var res = await _client.GetAsync($"/api/locations/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<LocationDto>();
        Assert.NotNull(body);
        Assert.Equal("Stockholm", body!.Name);
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsConflictProblemDetails()
    {
        await _factory.ResetDbAsync();

        _ = await _api.CreateLocationAsync("Uppsala");

        var res = await _client.PostAsJsonAsync("/api/locations", new CreateLocationRequest("Uppsala"));

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(409, body!.Status);
    }
}
