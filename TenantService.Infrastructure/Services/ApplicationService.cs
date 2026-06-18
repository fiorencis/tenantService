using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TenantService.Application;

namespace TenantService.Infrastructure;

public abstract class ApplicationService : IApplicationService
{
    protected readonly ILogger<ApplicationService> _logger;

    protected readonly IConfiguration _config;


    public ApplicationService(IConfiguration config, ILogger<ApplicationService> logger)
    {
        _logger = logger;
        _config = config;
    }


}
