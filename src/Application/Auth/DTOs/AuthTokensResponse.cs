namespace SalonCRM.Application.Auth.DTOs;

public class AuthTokensResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Access token lifetime in seconds, from the moment it is issued.
    /// </summary>
    public int ExpiresIn { get; set; }
}
