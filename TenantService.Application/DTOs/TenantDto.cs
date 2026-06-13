namespace TenantService.Application.DTOs;

public class TenantDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }  // Rappresentazione stringa dello status
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
