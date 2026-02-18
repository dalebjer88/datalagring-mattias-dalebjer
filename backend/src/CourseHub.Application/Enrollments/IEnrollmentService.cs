namespace CourseHub.Application.Enrollments;

public interface IEnrollmentService
{
    Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<EnrollmentDto>> GetAllAsync(CancellationToken ct = default);
    Task<EnrollmentDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EnrollmentDto> UpdateAsync(int id, UpdateEnrollmentRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
