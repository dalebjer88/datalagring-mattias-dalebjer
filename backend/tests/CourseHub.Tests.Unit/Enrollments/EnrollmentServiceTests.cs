using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.Enrollments;
using CourseHub.Domain.Entities;
using NSubstitute;

namespace CourseHub.Tests.Unit.Enrollments;

public sealed class EnrollmentServiceTests
{
    private readonly IEnrollmentRepository _repo = Substitute.For<IEnrollmentRepository>();
    private readonly EnrollmentService _sut;

    public EnrollmentServiceTests()
    {
        _sut = new EnrollmentService(_repo);
    }

    [Fact]
    public async Task CreateAsync_WhenParticipantMissing_ThrowsValidationException()
    {
        _repo.ParticipantExistsAsync(1, Arg.Any<CancellationToken>()).Returns(false);

        var req = new CreateEnrollmentRequest(1, 2, "Active");

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenEnrollmentExists_ThrowsConflictException()
    {
        _repo.ParticipantExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _repo.CourseInstanceExistsAsync(2, Arg.Any<CancellationToken>()).Returns(true);
        _repo.EnrollmentExistsAsync(1, 2, Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateEnrollmentRequest(1, 2, "Active");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ThrowsNotFoundException()
    {
        _repo.GetForUpdateAsync(5, Arg.Any<CancellationToken>()).Returns((Enrollment?)null);

        var req = new UpdateEnrollmentRequest("Active");

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(5, req));
    }
}
