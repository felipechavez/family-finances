namespace FinanceApp.Infrastructure;
using System.Text;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Infrastructure.Services;
using FinanceApp.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Extension methods for registering Infrastructure services into the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure-layer services: persistence, caching, authentication, and domain services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="config">The application configuration used for connection strings and JWT settings.</param>
    /// <returns>The configured <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Strongly-typed JWT settings with validation
        services.AddOptions<JwtSettings>()
            .Bind(config.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Email verification feature flag (set Enabled=false in dev to skip Resend)
        services.AddOptions<EmailVerificationOptions>()
            .Bind(config.GetSection(EmailVerificationOptions.SectionName));

        // Redis
        services.AddStackExchangeRedisCache(opts =>
            opts.Configuration = config.GetConnectionString("Redis"));

        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<ITotpService, TotpService>();

        // Email (MailerSend SMTP relay)
        services.AddOptions<MailerSendSettings>()
            .Bind(config.GetSection("MailerSend"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<IEmailService, MailerSendEmailService>();

        // JWT Authentication
        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                };
            });

        services.AddAuthorization();

        return services;
    }
}
