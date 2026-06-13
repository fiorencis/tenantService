using TenantService.Domain.Enums;

namespace TenantService.Domain.Entities;

public class User
{
    public User()
    {
        Id = Guid.NewGuid();

        var emptyuser = Id.ToString().Substring(0, 8);

        Username = $"user_{emptyuser}";
        FullName = $"User {emptyuser}"; 
        Email = $"user_{emptyuser}@example.com";   
    }

    public User(Guid id, string username, string fullname, string email) : this()
    {
        Id = id;
        Username = username;
        FullName = fullname;
        Email = email;
    }
    
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string PasswordHash { get; set; }

    public bool Admin { get; set; } = false;

    // Il password hashing deve essere eseguito da un servizio esterno
    // (es. IPasswordHasher / BCrypt) per mantenere il dominio indipendente.
}
