using CourseHub.Application.Participants;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public sealed class ParticipantRepository : BaseRepository<Participant>, IParticipantRepository
{
    public ParticipantRepository(CourseHubDbContext db) : base(db)
    {
    }

    public override async Task<IReadOnlyList<Participant>> GetAllAsync(CancellationToken ct = default)
    {
        return await Db.Participants
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(ct);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return Db.Participants.AnyAsync(x => x.Email == email, ct);
    }

    public Task<Participant?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return Db.Participants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }
}
