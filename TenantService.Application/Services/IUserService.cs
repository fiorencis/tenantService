using TenantService.Application.DTOs;

namespace TenantService.Application;

public interface IUserService : IApplicationService
{
	Task<Guid> AddUserAsync(UserDto user, CancellationToken cancellationToken = default);




}
