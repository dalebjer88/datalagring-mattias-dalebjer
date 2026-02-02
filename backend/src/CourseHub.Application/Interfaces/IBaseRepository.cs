namespace CourseHub.Application.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TEntity?> GetForUpdateAsync(int id, CancellationToken ct = default);
    Task RemoveAsync(TEntity entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
