namespace TenantService.Application;

public interface IInitService : IApplicationService
{
    Task<String> InitializeDatabaseAsync (CancellationToken cancellationToken = default);
}
