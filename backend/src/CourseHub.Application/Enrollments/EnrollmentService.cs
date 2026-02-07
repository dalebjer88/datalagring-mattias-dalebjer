using CourseHub.Domain.Entities;

namespace CourseHub.Application.Enrollments;

public sealed class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;

    public EnrollmentService(IEnrollmentRepository repo)
    {
        _repo = repo;
    }

    public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request, CancellationToken ct = default)
    {
        var participantId = request.ParticipantId;
        var courseInstanceId = request.CourseInstanceId;
        var status = request.Status.Trim();

        Validate(status);

        if (!await _repo.ParticipantExistsAsync(participantId, ct))
            throw new InvalidOperationException("Participant does not exist.");

        if (!await _repo.CourseInstanceExistsAsync(courseInstanceId, ct))
            throw new InvalidOperationException("Course instance does not exist.");

        if (await _repo.EnrollmentExistsAsync(participantId, courseInstanceId, ct))
            throw new InvalidOperationException("Enrollment already exists.");

        var enrollment = new Enrollment
        {
            ParticipantId = participantId,
            CourseInstanceId = courseInstanceId,
            RegisteredAt = DateTime.UtcNow,
            Status = status
        };

        await _repo.AddAsync(enrollment, ct);
        await _repo.SaveChangesAsync(ct);

        return new EnrollmentDto(enrollment.Id, enrollment.ParticipantId, enrollment.CourseInstanceId, enrollment.RegisteredAt, enrollment.Status);
    }

    public async Task<IReadOnlyList<EnrollmentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);

        return items
            .Select(x => new EnrollmentDto(x.Id, x.ParticipantId, x.CourseInstanceId, x.RegisteredAt, x.Status))
            .ToList();
    }

    public async Task<EnrollmentDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        if (item is null) return null;

        return new EnrollmentDto(item.Id, item.ParticipantId, item.CourseInstanceId, item.RegisteredAt, item.Status);
    }

    public async Task<EnrollmentDto?> UpdateAsync(int id, UpdateEnrollmentRequest request, CancellationToken ct = default)
    {
        var item = await _repo.GetForUpdateAsync(id, ct);
        if (item is null) return null;

        var status = request.Status.Trim();
        Validate(status);

        item.Status = status;

        await _repo.SaveChangesAsync(ct);

        return new EnrollmentDto(item.Id, item.ParticipantId, item.CourseInstanceId, item.RegisteredAt, item.Status);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.GetForUpdateAsync(id, ct);
        if (item is null) return false;

        await _repo.RemoveAsync(item, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }

    private static void Validate(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) throw new InvalidOperationException("Status is required.");
        if (status.Length > 30) throw new InvalidOperationException("Status is too long.");
    }
}
