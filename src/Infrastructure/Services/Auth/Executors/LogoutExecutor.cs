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

public class LogoutExecutor : AuthExecutorBase, ILogoutExecutor
{
    public LogoutExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
    }

    public async Task<GenericResponse> ExecuteAsync(Guid userId, RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var token = await DbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId == userId, cancellationToken);

        if (token is null)
        {
            throw new AppException("Refresh token not found.", AppErrorType.NotFound);
        }

        // Revoking every active token for the user makes logout end all sessions,
        // not just the one tied to the provided refresh token.
        await RevokeAllRefreshTokensAsync(userId, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("Logged out successfully.");
    }
}
