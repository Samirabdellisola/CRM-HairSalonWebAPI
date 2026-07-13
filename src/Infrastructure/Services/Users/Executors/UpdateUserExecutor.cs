using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Users.DTOs;
using SalonCRM.Application.Users.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Users.Executors;

public class UpdateUserExecutor : IUpdateUserExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IBranchScopeChecker _branchScopeChecker;

    public UpdateUserExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        _dbContext = dbContext;
        _branchScopeChecker = branchScopeChecker;
    }

    public async Task<UserDetailsResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid targetUserId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var targetUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == targetUserId, cancellationToken);
        if (targetUser is null)
        {
            throw new AppException("User not found.", AppErrorType.NotFound);
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            await EnsureBranchAdminCanManageAsync(callerId, targetUser.BranchId, targetUser.Role, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (normalizedEmail != targetUser.Email)
            {
                var emailTaken = await _dbContext.Users
                    .AnyAsync(u => u.Email == normalizedEmail && u.Id != targetUserId, cancellationToken);
                if (emailTaken)
                {
                    throw new AppException("Email is already registered.", AppErrorType.Conflict);
                }

                targetUser.Email = normalizedEmail;
            }
        }

        if (request.Phone is not null)
        {
            targetUser.Phone = request.Phone;
        }

        if (request.Address is not null)
        {
            targetUser.Address = request.Address;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDetailsResponse(targetUser);
    }

    private async Task EnsureBranchAdminCanManageAsync(
        Guid branchAdminId,
        Guid? targetBranchId,
        UserRole targetRole,
        CancellationToken cancellationToken)
    {
        if (targetRole != UserRole.Staff && targetRole != UserRole.Customer)
        {
            throw new AppException("BranchAdmin can only manage Staff or Customer users.", AppErrorType.Forbidden);
        }

        var administeredBranch = await _branchScopeChecker.GetAdministeredBranchAsync(branchAdminId, cancellationToken);
        if (administeredBranch is null || targetBranchId != administeredBranch.Id)
        {
            throw new AppException("BranchAdmin can only manage users within their own branch.", AppErrorType.Forbidden);
        }
    }

    private static UserDetailsResponse ToDetailsResponse(Domain.Entities.User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Role = user.Role,
        Phone = user.Phone,
        Address = user.Address,
        BranchId = user.BranchId
    };
}
