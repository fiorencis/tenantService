using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantService.API.Controllers;
using TenantService.Application;
using TenantService.Application.DTOs;
using TenantService.Application.Services;

namespace TenantService.API;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController : TenantBaseController
{
    protected readonly IUserService _userService;
    protected readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService, IConfiguration configuration, ILogger<TenantBaseController> logger) : base(configuration, logger)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    // Tenant service user login procedure by credentials validation 
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var result = await _userService.ValidateUserCredentialsAsync(request.Username, request.Password);

        if (!result.IsSuccess)
        {
            return Unauthorized(result.ErrorMessage);
        }

        var tokenpair = await _tokenService.CreateTokenPair(request.Username, "Admin");

        return Ok(tokenpair);
    }

    // Tenant service user logout procedure by refresh token invalidation
    [HttpPost("refresh")]
    [AllowAnonymous]    
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        var tokenpair = await _tokenService.RefreshTokenPairAsync(request.AccessToken, request.RefreshToken);





        return Ok(tokenpair);
    }

}
