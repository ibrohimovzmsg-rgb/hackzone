using HackZone.Application;
using HackZone.Domain.Interfaces;
using HackZone.Infrastructure.Persistence;
using HackZone.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minio;
using StackExchange.Redis;
using System.Text;

namespace HackZone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Application layer
        services.AddApplication();

        // Database
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // JWT Service
        services.AddScoped<IJwtService, JwtService>();

        // JWT Auth
        var jwtSecret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"] ?? "hackzone.uz",
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"] ?? "hackzone.uz",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // Redis
        var redisConn = config["Redis:ConnectionString"] ?? "redis:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConn));
        services.AddScoped<ICacheService, RedisCacheService>();

        // MinIO
        var minioEndpoint = config["Minio:Endpoint"] ?? "minio:9000";
        var minioAccess = config["Minio:AccessKey"] ?? "hackzone_minio";
        var minioSecret = config["Minio:SecretKey"] ?? "hackzone_secret";
        services.AddMinio(c => c
            .WithEndpoint(minioEndpoint)
            .WithCredentials(minioAccess, minioSecret)
            .WithSSL(false)
            .Build());
        services.AddScoped<IFileStorageService, MinioFileStorageService>();

        // Email
        services.AddScoped<IEmailService, EmailService>();

        // Docker Labs
        services.AddScoped<ILabOrchestrator, DockerLabOrchestrator>();

        // HTTP context
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
