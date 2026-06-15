namespace TenantService.Domain;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid userId)
        : base($"User with id '{userId}' was not found.")
    {
        UserId = userId;
    }

    public UserNotFoundException(string username) 
        : base($"User with username '{username}' was not found.")
    {
        Username = username;
    }

    public Guid UserId { get; }
    public string Username { get; }
}
