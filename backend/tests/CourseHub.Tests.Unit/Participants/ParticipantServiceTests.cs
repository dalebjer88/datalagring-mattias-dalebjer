using CourseHub.Application.Common.Exceptions;
using CourseHub.Application.Participants;
using CourseHub.Domain.Entities;
using NSubstitute;

namespace CourseHub.Tests.Unit.Participants;

public sealed class ParticipantServiceTests
{
    private readonly IParticipantRepository _repo = Substitute.For<IParticipantRepository>();
    private readonly ParticipantService _sut;

    public ParticipantServiceTests()
    {
        _sut = new ParticipantService(_repo);
    }

    [Fact]
    public async Task CreateAsync_NormalizesEmailToLower_AndTrims()
    {
        _repo.EmailExistsAsync("a@test.com", Arg.Any<CancellationToken>()).Returns(false);

        var req = new CreateParticipantRequest(" A@TEST.COM ", " Pat ", " Doe ", " 070 ");

        var dto = await _sut.CreateAsync(req);

        Assert.Equal("a@test.com", dto.Email);
        Assert.Equal("Pat", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("070", dto.PhoneNumber);

        await _repo.Received(1).AddAsync(
            Arg.Is<Participant>(p =>
                p.Email == "a@test.com" &&
                p.FirstName == "Pat" &&
                p.LastName == "Doe" &&
                p.PhoneNumber == "070"),
            Arg.Any<CancellationToken>());

        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ThrowsConflictException()
    {
        _repo.EmailExistsAsync("dup@test.com", Arg.Any<CancellationToken>()).Returns(true);

        var req = new CreateParticipantRequest(" dup@test.com ", "A", "B", "070");

        await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(req));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ThrowsNotFoundException()
    {
        _repo.GetForUpdateAsync(1, Arg.Any<CancellationToken>()).Returns((Participant?)null);

        var req = new UpdateParticipantRequest("a@test.com", "A", "B", "070");

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(1, req));
    }
}
