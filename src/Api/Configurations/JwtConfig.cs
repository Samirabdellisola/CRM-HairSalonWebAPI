using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SalonCRM.Application.Auth.Options;

namespace SalonCRM.Api.Configurations;

public static class JwtConfig
{
    /// <summary>
    /// Configures JWT Bearer authentication using Jwt:Key / Jwt:Issuer / Jwt:Audience
    /// from configuration (env vars Jwt__Key, Jwt__Issuer, Jwt__Audience on Render).
    /// The role claim type is mapped to "role" to match the claim emitted by
    /// JwtTokenService, so [Authorize(Roles = "CentralOffice,BranchAdmin")] works as expected.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Keep JWT claim names as emitted (e.g. "role"), so [Authorize(Roles = "...")] works.
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();

        return services;
    }
}
