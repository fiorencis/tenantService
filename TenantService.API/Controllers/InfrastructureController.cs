using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TenantService.Application;
using TenantService.Application.DTOs;

namespace TenantService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfrastructureController : TenantBaseController
{
    protected readonly IUserService _userService;

    public InfrastructureController(IUserService userService, ILogger<TenantBaseController> logger) : base(logger)
    {
        _userService = userService;
    }
    
    
    [HttpGet("info")]
    public async Task<IActionResult> Info()
    { 
        _logger.LogDebug("Service Info request");

        var assembly = Assembly.GetExecutingAssembly();

        var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;

        var version = assembly.GetName().Version;
        var title = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

        TenantInfoResult data = new TenantInfoResult($"{title}", $"{version}", "Servizi di gestione applicazioni Multi-Tenant", $"{copyright}");

        _logger.LogInformation(title, version, copyright);

        return Ok(data);
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser([FromBody] UserOperationRequest request)
    {
        _logger.LogWarning($"Adding new User {request.User.ToString()}");

        var userId = await _userService.AddUserAsync(request.User);

        return Ok(new { Id = userId });
    }

    // Tenant Management CRUD operations    
    // [HttpPost("add-tenant")]
    // public async Task<IActionResult> AddTenant(TenantOperationRequest request)
    // {
    //     _logger.LogDebug($"Adding new Tenant {request.Tenant.Id + "-" + request.Tenant.Name}");
    //     var tenantDbContext = _dbResolver.GetContextForTenant(request.TenantId);

    //     _vetcDbContext.Tenants.Add(request.Tenant);
    //     _vetcDbContext.SaveChanges();

    //     return Ok();
    // }
}
