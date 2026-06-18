using Microsoft.AspNetCore.Mvc;

namespace TenantService.API.Controllers;

public abstract class TenantBaseController : ControllerBase
{
    protected readonly IConfiguration _config;
    protected readonly ILogger<TenantBaseController> _logger;

    protected TenantBaseController(IConfiguration configuration, ILogger<TenantBaseController> logger)
    {
        _config = configuration;
        _logger = logger;
    }
}
