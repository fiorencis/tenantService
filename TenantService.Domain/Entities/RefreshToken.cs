namespace TenantService.Domain.Entities;

public class RefreshToken
{
    public int      Id          { get; set; }
    public string   Token       { get; set; } = default!;
    public string   Username    { get; set; } = default!;
    public DateTime ExpiresAt   { get; set; }
    public DateTime CreatedAt   { get; set; }
    public bool     IsRevoked   { get; set; }
}
