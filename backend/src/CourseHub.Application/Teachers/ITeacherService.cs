namespace CourseHub.Application.Teachers;

public interface ITeacherService
{
    Task<TeacherDto> CreateAsync(CreateTeacherRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TeacherDto>> GetAllAsync(CancellationToken ct = default);
    Task<TeacherDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TeacherDto> UpdateAsync(int id, UpdateTeacherRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
