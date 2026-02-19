using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.Locations;
using CourseHub.Domain.Entities;
using NSubstitute;

namespace CourseHub.Tests.Unit.Locations;

public sealed class LocationServiceTests
{
    private readonly ILocationRepository _repo = Substitute.For<ILocationRepository>();
    private readonly LocationService _sut;

    public LocationServiceTests()
    {
        _sut = new LocationService(_repo);
    }

    [Fact]
    public async Task CreateAsync_WhenNameMissing_ThrowsValidationException()
    {
        var req = new CreateLocationRequest("   ");

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenNameExists_ThrowsConflictException()
    {
        _repo.NameExistsAsync("Karlstad", Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateLocationRequest(" Karlstad ");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenUsedByCourseInstances_ThrowsConflictException()
    {
        var entity = new Location { Name = "X" };

        _repo.GetForUpdateAsync(3, Arg.Any<CancellationToken>()).Returns(entity);
        _repo.IsUsedByCourseInstancesAsync(3, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() => _sut.DeleteAsync(3));

        await _repo.DidNotReceive().RemoveAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
