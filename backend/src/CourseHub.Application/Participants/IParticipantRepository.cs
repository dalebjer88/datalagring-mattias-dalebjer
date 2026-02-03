using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Participants;

public interface IParticipantRepository : IBaseRepository<Participant>
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<Participant?> GetByEmailAsync(string email, CancellationToken ct = default);
}
