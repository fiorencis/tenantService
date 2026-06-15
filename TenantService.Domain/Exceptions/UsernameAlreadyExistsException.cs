namespace TenantService.Domain.Exceptions;

public sealed class UsernameAlreadyExistsException : Exception
{
	public UsernameAlreadyExistsException(string username)
		: base($"Username '{username}' is already in use.")
	{
		Username = username;
	}

	public string Username { get; }
}