using CourseHub.Application.Interfaces;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly CourseHubDbContext Db;
    protected readonly DbSet<TEntity> Set;

    protected BaseRepository(CourseHubDbContext db)
    {
        Db = db;
        Set = db.Set<TEntity>();
    }

    public virtual Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        Set.Add(entity);
        return Task.FromResult(entity);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await Set.AsNoTracking().ToListAsync(ct);
    }

    public virtual Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return Set.AsNoTracking()
            .FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id, ct);
    }

    public virtual Task<TEntity?> GetForUpdateAsync(int id, CancellationToken ct = default)
    {
        return Set.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id, ct);
    }

    public Task RemoveAsync(TEntity entity, CancellationToken ct = default)
    {
        Set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return Db.SaveChangesAsync(ct);
    }
}
