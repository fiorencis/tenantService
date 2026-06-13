using Microsoft.Extensions.Logging;
using TenantService.Application;

namespace TenantService.Infrastructure;

public abstract class ApplicationService : IApplicationService
{
    protected readonly ILogger<ApplicationService> _logger;

    public ApplicationService(ILogger<ApplicationService> logger)
    {
        _logger = logger;
    }


}
