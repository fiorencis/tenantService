using TenantService.API.Controllers.Core;
using TenantService.Application.DTOs;

namespace TenantService.API;

public class UserOperationRequest : ApiRequestBase
{
    public UserDto User { get; set; }
}
