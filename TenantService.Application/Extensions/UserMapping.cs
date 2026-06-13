using TenantService.Application.DTOs;
using TenantService.Domain.Entities;
using TenantService.Domain.Enums;

namespace TenantService.Application;

public static class UserMapping
{
    public static User ToCreateUser(this UserDto dto) => new()
    {
            Username = dto.Username,
            FullName = dto.FullName,
            Email = dto.Email,
            Admin = dto.Admin,
            Status = (UserStatus)dto.Status
    };
}
