using TenantService.Application.DTOs;
using TenantService.Domain.Entities;
using TenantService.Domain.Enums;

namespace TenantService.Application;

public static class UserMapping
{
    public static User ToCreateUser(this UserDto dto, string passwordHash) => new()
    {
            Username = dto.Username,
            FullName = dto.FullName,
            Email = dto.Email,
            Admin = dto.Admin,
            Status = (UserStatus)dto.Status,
            PasswordHash = passwordHash
    };

    public static UserDto ToUserDto(this User user) => new()
    {
        Id = user.Id.ToString(),
        Username = user.Username,
        FullName = user.FullName,
        Email = user.Email,
        Admin = user.Admin,
        Status = (int)user.Status,
        PasswordHash = user.PasswordHash
    };
}
