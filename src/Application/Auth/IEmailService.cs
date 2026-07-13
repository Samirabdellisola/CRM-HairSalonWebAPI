using SalonCRM.Domain.Entities;

namespace SalonCRM.Application.Auth;

/// <summary>
/// Abstraction for outbound transactional emails related to authentication.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends the password reset link to the user. Implementations must never
    /// include the user's current or previous password in the email.
    /// </summary>
    Task SendPasswordResetEmailAsync(
        User user,
        string resetToken,
        string resetLink,
        CancellationToken cancellationToken = default);
}
