using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.Courses;
using CourseHub.Domain.Entities;
using NSubstitute;

namespace CourseHub.Tests.Unit.Courses;

public sealed class CourseServiceTests
{
    private readonly ICourseRepository _repo = Substitute.For<ICourseRepository>();
    private readonly CourseService _sut;

    public CourseServiceTests()
    {
        _sut = new CourseService(_repo);
    }

    [Fact]
    public async Task CreateAsync_WhenCourseCodeExists_ThrowsConflictException()
    {
        _repo.CourseCodeExistsAsync("NET-1", Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateCourseRequest(" NET-1 ", " Title ", " Desc ");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_TrimsInput_WritesEntity_AndSaves()
    {
        _repo.CourseCodeExistsAsync("NET-2", Arg.Any<CancellationToken>()).Returns(false);
        _repo.AddAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>()).Returns(ci => ci.Arg<Course>());

        var req = new CreateCourseRequest(" NET-2 ", " Title ", " Desc ");

        var dto = await _sut.CreateAsync(req);

        Assert.Equal("NET-2", dto.CourseCode);
        Assert.Equal("Title", dto.Title);
        Assert.Equal("Desc", dto.Description);

        await _repo.Received(1).AddAsync(
            Arg.Is<Course>(c =>
                c.CourseCode == "NET-2" &&
                c.Title == "Title" &&
                c.Description == "Desc"),
            Arg.Any<CancellationToken>());

        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenUsedByCourseInstances_ThrowsConflictException()
    {
        var entity = new Course { CourseCode = "NET-3", Title = "T", Description = "D" };

        _repo.GetForUpdateAsync(5, Arg.Any<CancellationToken>()).Returns(entity);
        _repo.IsUsedByCourseInstancesAsync(5, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() => _sut.DeleteAsync(5));

        await _repo.DidNotReceive().RemoveAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
