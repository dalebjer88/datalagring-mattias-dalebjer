namespace CourseHub.Application.Locations;

public interface ILocationService
{
    Task<LocationDto> CreateAsync(CreateLocationRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken ct = default);
    Task<LocationDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LocationDto> UpdateAsync(int id, UpdateLocationRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
