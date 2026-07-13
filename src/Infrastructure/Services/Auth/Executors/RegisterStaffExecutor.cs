using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Auth.Executors;

public class RegisterStaffExecutor : AuthExecutorBase, IRegisterStaffExecutor
{
    private readonly IBranchScopeChecker _branchScopeChecker;

    public RegisterStaffExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher,
        IBranchScopeChecker branchScopeChecker)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
        _branchScopeChecker = branchScopeChecker;
    }

    public async Task<RegisterUserResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        RegisterStaffRequest request,
        CancellationToken cancellationToken = default)
    {
        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await _branchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null || administeredBranch.Id != request.BranchId)
            {
                throw new AppException("BranchAdmin can only register staff for their own branch.", AppErrorType.Forbidden);
            }
        }

        var email = NormalizeEmail(request.Email);
        await EnsureEmailIsAvailableAsync(email, cancellationToken);

        var user = new User
        {
            Email = email,
            Role = UserRole.Staff,
            BranchId = request.BranchId,
            IsActive = true
        };
        user.PasswordHash = PasswordHasher.HashPassword(user, request.Password);

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToRegisterResponse(user);
    }
}
