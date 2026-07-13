using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Entities;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure.Services.Auth.Executors;

public class RefreshTokenExecutor : AuthExecutorBase, IRefreshTokenExecutor
{
    public RefreshTokenExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
    }

    public async Task<AuthTokensResponse> ExecuteAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var existingToken = await DbContext.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

        var utcNow = DateTime.UtcNow;

        if (existingToken is null
            || existingToken.RevokedAt is not null
            || existingToken.ExpiresAt <= utcNow
            || existingToken.User is null
            || !existingToken.User.IsActive)
        {
            throw new AppException("Invalid or expired refresh token.", AppErrorType.Unauthorized);
        }

        existingToken.RevokedAt = utcNow;

        return await IssueTokensAsync(existingToken.User, cancellationToken);
    }
}
