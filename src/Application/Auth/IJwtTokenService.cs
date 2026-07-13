using SalonCRM.Domain.Entities;

namespace SalonCRM.Application.Auth;

/// <summary>
/// Issues short-lived signed JWT access tokens for authenticated users.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a signed access token with sub/email/role claims.
    /// Returns the token and its lifetime in seconds.
    /// </summary>
    (string AccessToken, int ExpiresInSeconds) GenerateAccessToken(User user);
}
