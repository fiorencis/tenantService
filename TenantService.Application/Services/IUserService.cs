using TenantService.Application.DTOs;

namespace TenantService.Application;

public interface IUserService : IApplicationService
{
	Task<Guid> AddUserAsync(UserDto user, CancellationToken cancellationToken = default);

	Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

	Task<UserDto> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
	
	Task<bool> ValidateUserCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);

}
