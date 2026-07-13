using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Domain.Entities;

namespace SalonCRM.Infrastructure.Services;

/// <summary>
/// Issues signed JWT access tokens. The role claim uses the short type "role"
/// so it lines up with JwtBearerOptions.TokenValidationParameters.RoleClaimType
/// configured in the Api layer, allowing [Authorize(Roles = "Admin")] to work.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public (string AccessToken, int ExpiresInSeconds) GenerateAccessToken(User user)
    {
        var expiresInSeconds = _jwtSettings.AccessTokenMinutes * 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (accessToken, expiresInSeconds);
    }
}
