using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TenantService.Application.Repositories;

namespace TenantService.Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly TenantDbContext DbContext;
    protected readonly DbSet<T> Set;

    public EfRepository(TenantDbContext dbContext)
    {
        DbContext = dbContext;
        Set = dbContext.Set<T>();
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Set.FindAsync([id], cancellationToken).AsTask();
    }

    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Set.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<List<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return Set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return Set.AnyAsync(predicate, cancellationToken);
    }

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        return Set.AddAsync(entity, cancellationToken).AsTask();
    }

    public void Update(T entity)
    {
        Set.Update(entity);
    }

    public void Remove(T entity)
    {
        Set.Remove(entity);
    }
}