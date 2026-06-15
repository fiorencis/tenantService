using Microsoft.Extensions.DependencyInjection;
using TenantService.Application.Repositories;
using TenantService.Application;
using TenantService.Infrastructure.Repositories;
using TenantService.Infrastructure.Security;
using TenantService.Infrastructure.Services;

namespace TenantService.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<Domain.Security.IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }
}