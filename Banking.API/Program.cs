using Microsoft.OpenApi.Models;
using Banking.Application.Interfaces;
using Banking.Infrastructure.Services;
using Banking.Persistence.Context;
using Banking.Persistence.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreRateLimit;
using Serilog;


using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/banking-log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add Controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Banking.API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT Token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// PostgreSQL Connection
builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddSingleton<IOtpService, OtpService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();



// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings["Secret"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],

            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey!))
        };
});

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));

builder.Services.AddInMemoryRateLimiting();

builder.Services.AddSingleton<IRateLimitConfiguration,
    RateLimitConfiguration>();

builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var logger = services
        .GetRequiredService<ILogger<Program>>();

    var dbContext = services
        .GetRequiredService<BankingDbContext>();

    var retries = 10;

    while (retries > 0)
    {
        try
        {
            logger.LogInformation(
                "Attempting database migration...");

            dbContext.Database.Migrate();

            logger.LogInformation(
                "Database migration successful.");

            break;
        }
        catch (Exception ex)
        {
            retries--;

            logger.LogError(
                ex,
                "Database migration failed. Retries left: {Retries}",
                retries);

            Thread.Sleep(5000);
        }
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();

app.UseIpRateLimiting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();