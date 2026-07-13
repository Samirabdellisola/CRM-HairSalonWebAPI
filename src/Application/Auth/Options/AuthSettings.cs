namespace SalonCRM.Application.Auth.Options;

/// <summary>
/// Bound from the "Auth" configuration section.
/// </summary>
public class AuthSettings
{
    public const string SectionName = "Auth";

    public int RefreshTokenDays { get; set; } = 14;

    public int PasswordResetTokenHours { get; set; } = 1;

    /// <summary>
    /// Base URL of the frontend reset-password page. The reset token is appended
    /// as a query string parameter: {FrontendResetPasswordBaseUrl}?token={token}
    /// </summary>
    public string FrontendResetPasswordBaseUrl { get; set; } = string.Empty;
}
