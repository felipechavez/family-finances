namespace FinanceApp.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Generates signed JWT tokens using the configured <see cref="JwtSettings"/>.
/// </summary>
public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _settings;
    private readonly ILogger<JwtService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="JwtService"/>.
    /// </summary>
    /// <param name="options">The JWT settings bound from configuration.</param>
    /// <param name="logger">The structured logger.</param>
    public JwtService(IOptions<JwtSettings> options, ILogger<JwtService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        _settings = options.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public string GenerateToken(Users user, Guid? familyId = null, string? role = null)
    {
        ArgumentNullException.ThrowIfNull(user);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(FamilyClaims.UserId, user.Id.ToString()),
            new(FamilyClaims.Email, user.Email),
            new(FamilyClaims.Name, user.Name),
            new(FamilyClaims.Jti, Guid.NewGuid().ToString())
        };

        if (familyId.HasValue)
            claims.Add(new Claim(FamilyClaims.FamilyId, familyId.Value.ToString()));
        if (!string.IsNullOrEmpty(role))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_settings.ExpirationDays),
            signingCredentials: creds);

        _logger.LogDebug("Generated JWT for user {UserId} with family {FamilyId}", user.Id, familyId);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc/>
    public string GenerateChallengeToken(Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(FamilyClaims.UserId, userId.ToString()),
            new Claim("scope", "2fa_challenge"),
            new Claim(FamilyClaims.Jti, Guid.NewGuid().ToString()),
        };
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc/>
    public Guid? ValidateChallengeToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            }, out var validated);

            var jwtToken = (JwtSecurityToken)validated;
            var scope = jwtToken.Claims.FirstOrDefault(c => c.Type == "scope")?.Value;
            if (scope != "2fa_challenge") return null;

            var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == FamilyClaims.UserId)?.Value;
            return Guid.TryParse(userIdStr, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }
}
