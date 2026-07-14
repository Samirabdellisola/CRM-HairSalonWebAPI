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

public class LoginExecutor : AuthExecutorBase, ILoginExecutor
{
    public LoginExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || !user.IsActive || user.IsFrozen)
        {
            throw new AppException("Invalid email or password.", AppErrorType.Unauthorized);
        }

        var verification = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new AppException("Invalid email or password.", AppErrorType.Unauthorized);
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = PasswordHasher.HashPassword(user, request.Password);
        }

        var tokens = await IssueTokensAsync(user, cancellationToken);

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            Tokens = tokens
        };
    }
}
