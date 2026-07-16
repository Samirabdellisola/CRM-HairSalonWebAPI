using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure.Services.Auth;

/// <summary>
/// Shared helpers used by every Auth executor: token issuing/rotation, email
/// normalization/uniqueness checks, and opaque token generation.
/// </summary>
public abstract class AuthExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IJwtTokenService JwtTokenService;
    protected readonly AuthSettings AuthSettings;
    protected readonly PasswordHasher<User> PasswordHasher;

    protected AuthExecutorBase(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
    {
        DbContext = dbContext;
        JwtTokenService = jwtTokenService;
        AuthSettings = authSettings.Value;
        PasswordHasher = passwordHasher;
    }

    protected static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    protected async Task EnsureEmailIsAvailableAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var exists = await DbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (exists)
        {
            throw new AppException("Email is already registered.", AppErrorType.Conflict);
        }
    }

    protected async Task<AuthTokensResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, expiresInSeconds) = JwtTokenService.GenerateAccessToken(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateOpaqueToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(AuthSettings.RefreshTokenDays)
        };

        DbContext.RefreshTokens.Add(refreshToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return new AuthTokensResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = expiresInSeconds
        };
    }

    protected async Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var activeTokens = await DbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = utcNow;
        }
    }

    /// <summary>
    /// Generates a cryptographically secure, URL-safe opaque token used for
    /// both refresh tokens and password reset tokens.
    /// </summary>
    protected static string GenerateOpaqueToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    protected static RegisterUserResponse ToRegisterResponse(User user) => new()
    {
        UserId = user.Id,
        Email = user.Email,
        Name = user.Name,
        Role = user.Role.ToString(),
        BranchId = user.BranchId
    };

    protected static void EnsureNonCentralOfficeHasBranch(User user)
    {
        if (user.Role != UserRole.CentralOffice && !user.BranchId.HasValue)
        {
            throw new AppException("User must be assigned to a branch.", AppErrorType.Validation);
        }
    }
}
