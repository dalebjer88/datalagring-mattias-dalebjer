using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.Teachers;
using CourseHub.Domain.Entities;
using NSubstitute;

namespace CourseHub.Tests.Unit.Teachers;

public sealed class TeacherServiceTests
{
    private readonly ITeacherRepository _repo = Substitute.For<ITeacherRepository>();
    private readonly TeacherService _sut;

    public TeacherServiceTests()
    {
        _sut = new TeacherService(_repo);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ThrowsConflictException()
    {
        _repo.EmailExistsAsync("t@test.com", Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateTeacherRequest(" T@TEST.COM ", "A", "B", "C#");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Teacher>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_NormalizesEmail_AndSaves()
    {
        _repo.EmailExistsAsync("x@test.com", Arg.Any<CancellationToken>()).Returns(false);

        var req = new CreateTeacherRequest(" X@TEST.COM ", " A ", " B ", " SQL ");

        var dto = await _sut.CreateAsync(req);

        Assert.Equal("x@test.com", dto.Email);
        Assert.Equal("A", dto.FirstName);
        Assert.Equal("B", dto.LastName);
        Assert.Equal("SQL", dto.Expertise);

        await _repo.Received(1).AddAsync(
            Arg.Is<Teacher>(t =>
                t.Email == "x@test.com" &&
                t.FirstName == "A" &&
                t.LastName == "B" &&
                t.Expertise == "SQL"),
            Arg.Any<CancellationToken>());

        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenUsedByCourseInstances_ThrowsConflictException()
    {
        var entity = new Teacher { Email = "a@test.com", FirstName = "A", LastName = "B", Expertise = "X" };

        _repo.GetForUpdateAsync(2, Arg.Any<CancellationToken>()).Returns(entity);
        _repo.IsUsedByCourseInstancesAsync(2, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() => _sut.DeleteAsync(2));

        await _repo.DidNotReceive().RemoveAsync(Arg.Any<Teacher>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
