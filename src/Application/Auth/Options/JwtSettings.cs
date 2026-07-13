namespace SalonCRM.Application.Auth.Options;

/// <summary>
/// Bound from the "Jwt" configuration section. Values come from environment
/// variables in production (e.g. Render: Jwt__Key, Jwt__Issuer, Jwt__Audience).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int AccessTokenMinutes { get; set; } = 15;
}
