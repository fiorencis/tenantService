using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TenantService.Application.DTOs;
using TenantService.Application.Repositories;
using TenantService.Domain.Entities;
using TenantService.Infrastructure;

namespace TenantService.Application.Services;

public class TokenService : ApplicationService, ITokenService
{
    private readonly IRepository<RefreshToken> _tokenRepository;
	private readonly IUnitOfWork _unitOfWork;

   
    private readonly TenantDbContext _tenantDbContext;

    public TokenService(IRepository<RefreshToken> tokenRepository, IUnitOfWork unitOfWork, 
        IConfiguration config, ILogger<TokenService> logger) : base(config, logger)
    {
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    } 

    public string GenerateAccessToken(string username, string role)
    {
        var jwt    = _config.GetSection("Jwt");
        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiryMinutes"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name,              username),
            new Claim(ClaimTypes.Role,              role)
        };

        var token = new JwtSecurityToken(
            issuer:             jwt["Issuer"],
            audience:           jwt["Audience"],
            claims:             claims,
            expires:            expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(string username)
    {
        var jwt = _config.GetSection("Jwt");

        return new RefreshToken
        {
            Token     = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Username  = username,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiryMinutes"]!)),
            IsRevoked = false
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwt = _config.GetSection("Jwt");

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = false, // ← ignora la scadenza intenzionalmente
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwt["Issuer"],
            ValidAudience            = jwt["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };

        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token, validationParams, out var validatedToken);

        // Assicurati che sia effettivamente un JWT con HMAC-SHA256
        if (validatedToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            return null;

        return principal;
    }

    public async Task<TokenPairDto> CreateTokenPair(string username, string role, CancellationToken cancellationToken = default)
    {
        var accessToken  = GenerateAccessToken(username, role);
        var refreshToken = GenerateRefreshToken(username);

        _tokenRepository.AddAsync(refreshToken, cancellationToken);
		_unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenPairDto(accessToken, refreshToken.Token);
    }

}