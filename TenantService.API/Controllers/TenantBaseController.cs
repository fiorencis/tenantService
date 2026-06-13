using Microsoft.AspNetCore.Mvc;

namespace TenantService.API.Controllers;

public abstract class TenantBaseController : ControllerBase
{
    protected readonly IConfiguration _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    protected readonly ILogger<TenantBaseController> _logger;

    public TenantBaseController(ILogger<TenantBaseController> logger)
    {
        _logger = logger;
    }
}
