using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.CourseInstances;
using CourseHub.Application.Enrollments;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Registrations;
using CourseHub.Domain.Entities;
using NSubstitute;

namespace CourseHub.Tests.Unit.Registrations;

public sealed class RegistrationServiceTests
{
    private readonly ICourseInstanceRepository _courseInstanceRepo = Substitute.For<ICourseInstanceRepository>();
    private readonly IEnrollmentRepository _enrollmentRepo = Substitute.For<IEnrollmentRepository>();
    private readonly ITransactionRunner _tx = Substitute.For<ITransactionRunner>();
    private readonly RegistrationService _sut;

    public RegistrationServiceTests()
    {
        _sut = new RegistrationService(_courseInstanceRepo, _enrollmentRepo, _tx);

        _tx.ExecuteAsync(Arg.Any<Func<CancellationToken, Task<RegistrationResultDto>>>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var fn = ci.Arg<Func<CancellationToken, Task<RegistrationResultDto>>>();
                return fn(CancellationToken.None);
            });
    }

    [Fact]
    public async Task CreateCourseInstanceWithEnrollmentsAsync_WhenTeacherIdsMissing_ThrowsValidationException()
    {
        var req = new CreateCourseInstanceWithEnrollmentsRequest(
            new DateOnly(2030, 1, 1),
            new DateOnly(2030, 1, 2),
            10,
            1,
            1,
            Array.Empty<int>(),
            new[] { 1 },
            "Active");

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateCourseInstanceWithEnrollmentsAsync(req));

        await _courseInstanceRepo.DidNotReceive().AddAsync(Arg.Any<CourseInstance>(), Arg.Any<CancellationToken>());
        await _enrollmentRepo.DidNotReceive().AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
        await _courseInstanceRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseInstanceWithEnrollmentsAsync_WhenParticipantMissing_ThrowsValidationException_AndDoesNotWrite()
    {
        _courseInstanceRepo.CourseExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _courseInstanceRepo.LocationExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _courseInstanceRepo.TeachersExistAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(true);

        _enrollmentRepo.ParticipantExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

        var req = new CreateCourseInstanceWithEnrollmentsRequest(
            new DateOnly(2030, 1, 1),
            new DateOnly(2030, 1, 2),
            10,
            1,
            1,
            new[] { 5 },
            new[] { 999 },
            "Active");

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateCourseInstanceWithEnrollmentsAsync(req));

        await _courseInstanceRepo.DidNotReceive().AddAsync(Arg.Any<CourseInstance>(), Arg.Any<CancellationToken>());
        await _enrollmentRepo.DidNotReceive().AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
        await _courseInstanceRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseInstanceWithEnrollmentsAsync_HappyPath_WritesAndSaves()
    {
        _courseInstanceRepo.CourseExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _courseInstanceRepo.LocationExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _courseInstanceRepo.TeachersExistAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(true);

        _enrollmentRepo.ParticipantExistsAsync(10, Arg.Any<CancellationToken>()).Returns(true);
        _enrollmentRepo.ParticipantExistsAsync(11, Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateCourseInstanceWithEnrollmentsRequest(
            new DateOnly(2030, 1, 1),
            new DateOnly(2030, 1, 2),
            10,
            1,
            1,
            new[] { 5 },
            new[] { 10, 11 },
            "Active");

        var result = await _sut.CreateCourseInstanceWithEnrollmentsAsync(req);

        Assert.Equal(2, result.EnrollmentCount);

        await _tx.Received(1).ExecuteAsync(Arg.Any<Func<CancellationToken, Task<RegistrationResultDto>>>(), Arg.Any<CancellationToken>());
        await _courseInstanceRepo.Received(1).AddAsync(Arg.Any<CourseInstance>(), Arg.Any<CancellationToken>());
        await _enrollmentRepo.Received(2).AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
        await _courseInstanceRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
