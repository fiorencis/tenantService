using TenantService.Application.Repositories;

namespace TenantService.Infrastructure.Repositories;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly TenantDbContext DbContext;

    public EfUnitOfWork(TenantDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.SaveChangesAsync(cancellationToken);
    }
}