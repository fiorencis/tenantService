using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace TenantService.Infrastructure.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<TokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation($"Executing 'Token Cleanup Service' on {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}");

        try 
        {

            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), ct);

                _logger.LogInformation($"Deleting elapsed tokens...");

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

                var expired = db.RefreshTokens
                    .Where(r => r.ExpiresAt < DateTime.UtcNow || r.IsRevoked);

                db.RefreshTokens.RemoveRange(expired);
                await db.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex) when (IsDatabaseMissingException(ex))
        {
            _logger.LogWarning($"Database not found. Skipping 'Token Cleanup Service' execution on {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    private static bool IsDatabaseMissingException(Exception ex)
    {
        // Controlla l'eccezione interna se EF Core ha incapsulato l'errore di Postgres
        var current = ex;

        while (current != null)
        {
            if (current is PostgresException pgEx && pgEx.SqlState == "3D000")
            {
                return true;
            }

            current = current.InnerException;
        }
        return false;
    }

}
