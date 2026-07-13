using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Entities;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure.Services.Auth.Executors;

public class ResetPasswordExecutor : AuthExecutorBase, IResetPasswordExecutor
{
    public ResetPasswordExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
    }

    public async Task<GenericResponse> ExecuteAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var resetToken = await DbContext.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token, cancellationToken);

        var utcNow = DateTime.UtcNow;

        if (resetToken is null
            || resetToken.UsedAt is not null
            || resetToken.ExpiresAt <= utcNow
            || resetToken.User is null)
        {
            throw new AppException("Invalid or expired reset token.", AppErrorType.Unauthorized);
        }

        var user = resetToken.User;
        user.PasswordHash = PasswordHasher.HashPassword(user, request.NewPassword);
        resetToken.UsedAt = utcNow;

        await RevokeAllRefreshTokensAsync(user.Id, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("Password has been reset successfully.");
    }
}
