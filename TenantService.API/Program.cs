// create the configuration file
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TenantService.Infrastructure;
using TenantService.Infrastructure.Extensions;
using TenantService.Application.Extensions;
using TenantService.API.Infrastructure;

// gets appsettings configuration
IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

// create the web application builder
var builder = WebApplication.CreateBuilder(args);

//Gets Connection String from environment variable or appsettings.json
var conStr = Environment.GetEnvironmentVariable("CONNECTION_STRING")
           ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Initialize Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/tnt-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, 
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

// add OpenApi services
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var secret = config.GetValue<string>("TokenKey") ?? "supersecretkey1234567890!@#$%^&*()";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "TenantServiceApi",
        ValidAudience = "TenantServiceClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Host.UseSerilog();

builder.Services.AddDbContext<TenantDbContext>(options => options.UseNpgsql(conStr));
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
