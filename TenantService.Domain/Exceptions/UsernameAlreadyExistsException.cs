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


public sealed class LoginCredentialsException : Exception
{
	public LoginCredentialsException(string username) 
		: base($"Invalid login credentials for user {username}")
	{
	}
}