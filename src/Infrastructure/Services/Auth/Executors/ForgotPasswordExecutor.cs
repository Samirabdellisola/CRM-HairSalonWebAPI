using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Domain.Entities;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure.Services.Auth.Executors;

public class ForgotPasswordExecutor : AuthExecutorBase, IForgotPasswordExecutor
{
    private readonly IEmailService _emailService;

    public ForgotPasswordExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher,
        IEmailService emailService)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
        _emailService = emailService;
    }

    public async Task<GenericResponse> ExecuteAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Always return the same generic response so callers cannot enumerate registered emails.
        const string genericMessage = "If the email is registered, a password reset link has been sent.";

        if (user is null || !user.IsActive)
        {
            return GenericResponse.Ok(genericMessage);
        }

        var token = GenerateOpaqueToken();
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(AuthSettings.PasswordResetTokenHours)
        };

        DbContext.PasswordResetTokens.Add(resetToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        var resetLink = $"{AuthSettings.FrontendResetPasswordBaseUrl}?token={token}";
        await _emailService.SendPasswordResetEmailAsync(user, token, resetLink, cancellationToken);

        return GenericResponse.Ok(genericMessage);
    }
}
