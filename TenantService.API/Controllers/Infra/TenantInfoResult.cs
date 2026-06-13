using TenantService.API.Controllers.Core;

namespace TenantService.API;

public class TenantInfoResult: ApiResponseBase
{
    public string Name { get; set; }
    public string Version { get; set;}
    public string Description { get; set;}
    public string Copyright { get; set;}

    public TenantInfoResult(string name, string version, string description, string copyright) 
    {
        Name = name;
        Version = version;
        Description = description;
        Copyright = copyright;
    }
}
