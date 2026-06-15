using Microsoft.Extensions.Logging;
using TenantService.Application;
using TenantService.Application.DTOs;
using TenantService.Application.Repositories;
using TenantService.Domain;
using TenantService.Domain.Entities;
using TenantService.Domain.Exceptions;
using TenantService.Domain.Security;

namespace TenantService.Infrastructure.Services;

public class UserService : ApplicationService, IUserService
{
	private readonly IRepository<User> _userRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IPasswordHasher _passwordHasher;

	public UserService(IRepository<User> userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ILogger<UserService> logger) : base(logger)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_passwordHasher = passwordHasher;
	}

	public async Task<Guid> AddUserAsync(UserDto user, CancellationToken cancellationToken = default)
	{
		if (!Guid.TryParse(user.Id, out var userId))
		{
			throw new ArgumentException("Invalid user id.", nameof(user));
		}

		var normalizedUsername = user.Username?.Trim();

		if (string.IsNullOrWhiteSpace(normalizedUsername))
		{
			throw new ArgumentException("Username is required.", nameof(user));
		}

		if (string.IsNullOrWhiteSpace(user.Password))
		{
			throw new ArgumentException("Password is required.", nameof(user));
		}

		if (await _userRepository.ExistsAsync(x => x.Username == normalizedUsername, cancellationToken))
		{
			throw new UsernameAlreadyExistsException(normalizedUsername);
		}

		var passwordHash = _passwordHasher.HashPassword(user.Password);
		var newUser = user.ToCreateUser(passwordHash);
		newUser.Id = userId;
		newUser.Username = normalizedUsername;
		
		await _userRepository.AddAsync(newUser, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return newUser.Id;
	}

	public async Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

		if (user == null)
		{
			throw new UserNotFoundException(userId);
		}

		return user.ToUserDto();
	}

	public async Task<UserDto> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		var normalizedUsername = username?.Trim();

		if (string.IsNullOrWhiteSpace(normalizedUsername))
		{
			throw new ArgumentException("Username is required.", nameof(username));
		}


		var user = await _userRepository.ListAsync(x => x.Username == normalizedUsername, cancellationToken);

		if (user == null || user.Count == 0)
		{
			throw new UserNotFoundException(normalizedUsername);
		}

		return user.FirstOrDefault()!.ToUserDto();
	}

	public async Task<bool> ValidateUserCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
	{
		var normalizedUsername = username?.Trim();

		if (string.IsNullOrWhiteSpace(normalizedUsername))
		{
			throw new ArgumentException("Username is required.", nameof(username));
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			throw new ArgumentException("Password is required.", nameof(password));
		}

		var user = await _userRepository.ListAsync(x => x.Username == normalizedUsername, cancellationToken);

		if (user == null || user.Count == 0)
		{
			return false;
		}

		var existingUser = user.FirstOrDefault()!;

		return _passwordHasher.VerifyPassword(password, existingUser.PasswordHash);
	}



}
