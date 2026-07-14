using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Users.DTOs;
using SalonCRM.Application.Users.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Users.Executors;

public class UpdateUserRoleExecutor : IUpdateUserRoleExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IBranchScopeChecker _branchScopeChecker;

    public UpdateUserRoleExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        _dbContext = dbContext;
        _branchScopeChecker = branchScopeChecker;
    }

    public async Task<UserDetailsResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid targetUserId,
        UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.NewRole == UserRole.CentralOffice)
        {
            throw new AppException("A user's role cannot be changed to CentralOffice.", AppErrorType.Validation);
        }

        if (request.NewRole == UserRole.BranchAdmin)
        {
            throw new AppException(
                "BranchAdmin can only be assigned via creating a branch or assign-admin.",
                AppErrorType.Validation);
        }

        var targetUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == targetUserId, cancellationToken);
        if (targetUser is null)
        {
            throw new AppException("User not found.", AppErrorType.NotFound);
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            if (request.NewRole != UserRole.Staff && request.NewRole != UserRole.Customer)
            {
                throw new AppException("BranchAdmin can only set the Staff or Customer role.", AppErrorType.Forbidden);
            }

            var administeredBranch = await _branchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null || targetUser.BranchId != administeredBranch.Id)
            {
                throw new AppException("BranchAdmin can only manage users within their own branch.", AppErrorType.Forbidden);
            }
        }

        targetUser.Role = request.NewRole;

        var utcNow = DateTime.UtcNow;
        var activeTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == targetUserId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = utcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserDetailsResponse
        {
            Id = targetUser.Id,
            Email = targetUser.Email,
            Role = targetUser.Role,
            Phone = targetUser.Phone,
            Address = targetUser.Address,
            BranchId = targetUser.BranchId
        };
    }
}
