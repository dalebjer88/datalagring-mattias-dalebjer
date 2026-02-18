using CourseHub.Application.Common.Exceptions;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Participants;

public sealed class ParticipantService : IParticipantService
{
    private readonly IParticipantRepository _repo;

    public ParticipantService(IParticipantRepository repo)
    {
        _repo = repo;
    }

    public async Task<ParticipantDto> CreateAsync(CreateParticipantRequest request, CancellationToken ct = default)
    {
        var email = NormalizeEmail(request.Email);
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        Validate(email, firstName, lastName, phoneNumber);

        if (await _repo.EmailExistsAsync(email, ct))
            throw new ConflictException("Email already exists.");

        var participant = new Participant
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber
        };

        await _repo.AddAsync(participant, ct);
        await _repo.SaveChangesAsync(ct);

        return new ParticipantDto(participant.Id, participant.Email, participant.FirstName, participant.LastName, participant.PhoneNumber);
    }

    public async Task<IReadOnlyList<ParticipantDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);

        return items
            .Select(x => new ParticipantDto(x.Id, x.Email, x.FirstName, x.LastName, x.PhoneNumber))
            .ToList();
    }

    public async Task<ParticipantDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var participant = await _repo.GetByIdAsync(id, ct);
        if (participant is null) throw new NotFoundException("Participant not found.");

        return new ParticipantDto(participant.Id, participant.Email, participant.FirstName, participant.LastName, participant.PhoneNumber);
    }

    public async Task<ParticipantDto> UpdateAsync(int id, UpdateParticipantRequest request, CancellationToken ct = default)
    {
        var participant = await _repo.GetForUpdateAsync(id, ct);
        if (participant is null) throw new NotFoundException("Participant not found.");

        var email = NormalizeEmail(request.Email);
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        Validate(email, firstName, lastName, phoneNumber);

        if (!string.Equals(participant.Email, email, StringComparison.Ordinal))
        {
            var existing = await _repo.GetByEmailAsync(email, ct);
            if (existing is not null && existing.Id != id)
                throw new ConflictException("Email already exists.");
        }

        participant.Email = email;
        participant.FirstName = firstName;
        participant.LastName = lastName;
        participant.PhoneNumber = phoneNumber;

        await _repo.SaveChangesAsync(ct);

        return new ParticipantDto(participant.Id, participant.Email, participant.FirstName, participant.LastName, participant.PhoneNumber);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var participant = await _repo.GetForUpdateAsync(id, ct);
        if (participant is null) throw new NotFoundException("Participant not found.");

        await _repo.RemoveAsync(participant, ct);
        await _repo.SaveChangesAsync(ct);
    }

    private static string NormalizeEmail(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static void Validate(string email, string firstName, string lastName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(firstName)) throw new ValidationException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ValidationException("Last name is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ValidationException("Phone number is required.");

        if (email.Length > 254) throw new ValidationException("Email is too long.");
        if (firstName.Length > 100) throw new ValidationException("First name is too long.");
        if (lastName.Length > 100) throw new ValidationException("Last name is too long.");
        if (phoneNumber.Length > 30) throw new ValidationException("Phone number is too long.");
    }
}
