using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.CourseInstances;
using CourseHub.Domain.Entities;
using NSubstitute;

namespace CourseHub.Tests.Unit.CourseInstances;

public sealed class CourseInstanceServiceTests
{
    private readonly ICourseInstanceRepository _repo = Substitute.For<ICourseInstanceRepository>();
    private readonly CourseInstanceService _sut;

    public CourseInstanceServiceTests()
    {
        _sut = new CourseInstanceService(_repo);
    }

    [Fact]
    public async Task CreateAsync_WhenNoTeachers_ThrowsValidationException()
    {
        _repo.CourseExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _repo.LocationExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateCourseInstanceRequest(
            new DateOnly(2030, 1, 1),
            new DateOnly(2030, 1, 2),
            10,
            1,
            1,
            Array.Empty<int>());

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(req));
    }

    [Fact]
    public async Task CreateAsync_WhenCourseMissing_ThrowsValidationException()
    {
        _repo.CourseExistsAsync(1, Arg.Any<CancellationToken>()).Returns(false);

        var req = new CreateCourseInstanceRequest(
            new DateOnly(2030, 1, 1),
            new DateOnly(2030, 1, 2),
            10,
            1,
            1,
            new[] { 1 });

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<CourseInstance>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_DistinctTeacherIds_AreAdded_AndSaved()
    {
        _repo.CourseExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _repo.LocationExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _repo.TeachersExistAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateCourseInstanceRequest(
            new DateOnly(2030, 1, 1),
            new DateOnly(2030, 1, 2),
            10,
            1,
            1,
            new[] { 2, 2, 3 });

        var dto = await _sut.CreateAsync(req);

        Assert.Equal(new[] { 2, 3 }, dto.TeacherIds.OrderBy(x => x).ToArray());

        await _repo.Received(1).AddAsync(
            Arg.Is<CourseInstance>(ci =>
                ci.CourseId == 1 &&
                ci.LocationId == 1 &&
                ci.CourseInstanceTeachers.Select(x => x.TeacherId).Distinct().Count() == 2),
            Arg.Any<CancellationToken>());

        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
