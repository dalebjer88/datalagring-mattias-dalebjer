namespace CourseHub.Application.Participants;

public interface IParticipantService
{
    Task<ParticipantDto> CreateAsync(CreateParticipantRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<ParticipantDto>> GetAllAsync(CancellationToken ct = default);
    Task<ParticipantDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ParticipantDto?> UpdateAsync(int id, UpdateParticipantRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
