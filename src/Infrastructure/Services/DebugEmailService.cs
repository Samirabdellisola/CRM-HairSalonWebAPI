using Microsoft.Extensions.Logging;
using SalonCRM.Application.Auth;
using SalonCRM.Domain.Entities;

namespace SalonCRM.Infrastructure.Services;

/// <summary>
/// Concrete IEmailService used until a real SMTP/email provider is wired up.
/// Logs the reset intent and link instead of sending an actual email. Never
/// logs the token value on its own outside of the full reset link.
/// </summary>
public class DebugEmailService : IEmailService
{
    private readonly ILogger<DebugEmailService> _logger;

    public DebugEmailService(ILogger<DebugEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(
        User user,
        string resetToken,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Password reset email requested for user {UserId}. Reset link: {ResetLink}",
            user.Id,
            resetLink);

        return Task.CompletedTask;
    }
}
