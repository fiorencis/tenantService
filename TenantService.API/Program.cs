using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TenantService.Infrastructure;
using TenantService.Infrastructure.Extensions;
using TenantService.Application.Extensions;
using TenantService.Infrastructure.Services;
using Microsoft.OpenApi;

// create the web application builder
var builder = WebApplication.CreateBuilder(args);

// create the configuration file
IConfiguration config = builder.Configuration;

//Gets Connection String from environment variable or appsettings.json
var conStr = Environment.GetEnvironmentVariable("CONNECTION_STRING")
           ?? config.GetConnectionString("DefaultConnection");

// Initialize Serilog logger
var logsec = config.GetSection("Logging");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File($"{logsec["LogPath"]}/tnt-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

// add OpenApi services
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Inserisci il token JWT nel formato: Bearer {token}"
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var secret = Environment.GetEnvironmentVariable("TOKEN_KEY") ??
        config.GetValue<string>("Key") ??
        "supersecretkey1234567890!@#$%^&*()";

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

builder.Services.AddHostedService<TokenCleanupService>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
//builder.Services.AddExceptionHandler<ApiExceptionHandler>();

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

var supportedCultures = new[]
{
    "en-US",
    "it-IT"
};

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.Run();
            