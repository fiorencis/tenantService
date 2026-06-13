using TenantService.Domain.Enums;

namespace TenantService.Domain.Entities;

// Entity che rappresenta un Tenant nel sistema
public class Tenant 
{
    public Tenant()
    {
        Users = new List<User>();
        Id = $@"tnn_{Guid.NewGuid().ToString()}";
        Name = $@"Tenant {Id}";
    }

    public Tenant(string id, string name, string description, TenantStatus status) : this()
    {
        Id = id;
        Name = name;
        Description = description;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    public Tenant(string id, string name, string description, TenantStatus status, DateTime createdAt) : this(id, name, description, status)
    {
        CreatedAt = createdAt;
    }
    
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TenantStatus Status { get; set; }  // Enum dal dominio
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Logica di dominio e validazioni
    public bool IsActive => Status == TenantStatus.Active;
    
    // Relazioni con altri entities
    public ICollection<User> Users { get; set; }
}
