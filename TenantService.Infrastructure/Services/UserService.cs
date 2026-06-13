using Microsoft.Extensions.Logging;
using TenantService.Application;
using TenantService.Application.DTOs;
using TenantService.Application.Repositories;
using TenantService.Domain.Entities;

namespace TenantService.Infrastructure.Services;

public class UserService : ApplicationService, IUserService
{
	private readonly IRepository<User> _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UserService(IRepository<User> userRepository, IUnitOfWork unitOfWork, ILogger<UserService> logger) : base(logger)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Guid> AddUserAsync(UserDto user, CancellationToken cancellationToken = default)
	{
		if (!Guid.TryParse(user.Id, out var userId))
		{
			throw new ArgumentException("Invalid user id.", nameof(user));
		}

		var newUser = user.ToCreateUser();
		newUser.Id = userId;
		
		await _userRepository.AddAsync(newUser, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return newUser.Id;
	}
}
