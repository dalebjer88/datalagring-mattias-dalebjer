using CourseHub.Application.Courses;
using CourseHub.Application.Locations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CourseHub.Tests.Integration.Caching;

public sealed class CacheInvalidationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public CacheInvalidationTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Courses_GetAll_SetsCache_And_Create_InvalidatesCache()
    {
        await _factory.ResetDbAsync();

        Assert.False(TryGetCache("courses:all", out _));

        var get1 = await _client.GetAsync("/api/courses");
        Assert.Equal(HttpStatusCode.OK, get1.StatusCode);

        Assert.True(TryGetCache("courses:all", out _));

        var create = new CreateCourseRequest("CACHE-101", "Cache Course", "Used for cache invalidation test");
        var post = await _client.PostAsJsonAsync("/api/courses", create);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        Assert.False(TryGetCache("courses:all", out _));

        var get2 = await _client.GetAsync("/api/courses");
        Assert.Equal(HttpStatusCode.OK, get2.StatusCode);

        Assert.True(TryGetCache("courses:all", out var cached));
        Assert.NotNull(cached);
    }

    [Fact]
    public async Task Locations_GetAll_SetsCache_And_Create_InvalidatesCache()
    {
        await _factory.ResetDbAsync();

        Assert.False(TryGetCache("locations:all", out _));

        var get1 = await _client.GetAsync("/api/locations");
        Assert.Equal(HttpStatusCode.OK, get1.StatusCode);

        Assert.True(TryGetCache("locations:all", out _));

        var create = new CreateLocationRequest("Cache Location");
        var post = await _client.PostAsJsonAsync("/api/locations", create);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        Assert.False(TryGetCache("locations:all", out _));

        var get2 = await _client.GetAsync("/api/locations");
        Assert.Equal(HttpStatusCode.OK, get2.StatusCode);

        Assert.True(TryGetCache("locations:all", out var cached));
        Assert.NotNull(cached);
    }

    private bool TryGetCache(string key, out object? value)
    {
        using var scope = _factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        return cache.TryGetValue(key, out value);
    }
}
