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

public class ChangePasswordExecutor : AuthExecutorBase, IChangePasswordExecutor
{
    public ChangePasswordExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
    }

    public async Task<GenericResponse> ExecuteAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new AppException("User not found.", AppErrorType.NotFound);
        }

        var verification = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new AppException("Current password is incorrect.", AppErrorType.Unauthorized);
        }

        user.PasswordHash = PasswordHasher.HashPassword(user, request.NewPassword);

        await RevokeAllRefreshTokensAsync(user.Id, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("Password changed successfully.");
    }
}
