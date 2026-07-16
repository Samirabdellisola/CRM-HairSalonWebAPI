using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches.Executors;

public class AssignBranchAdminExecutor : BranchExecutorBase, IAssignBranchAdminExecutor
{
    public AssignBranchAdminExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        AssignBranchAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanManageBranchAsync(callerId, callerRole, branchId, cancellationToken);

        var branch = await BranchesWithAdmin.FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken);
        if (branch is null)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        var targetUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (targetUser is null)
        {
            throw new AppException("User not found.", AppErrorType.NotFound);
        }

        if (targetUser.Role != UserRole.Staff || targetUser.BranchId != branchId)
        {
            throw new AppException("Only Staff belonging to this branch can be assigned as BranchAdmin.", AppErrorType.Validation);
        }

        if (branch.AdminId.HasValue && branch.AdminId.Value != targetUser.Id)
        {
            var previousAdmin = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == branch.AdminId.Value, cancellationToken);
            if (previousAdmin is not null)
            {
                previousAdmin.Role = UserRole.Staff;
                await RevokeAllRefreshTokensAsync(previousAdmin.Id, cancellationToken);
            }
        }

        targetUser.Role = UserRole.BranchAdmin;
        targetUser.BranchId = branchId;
        branch.AdminId = targetUser.Id;
        branch.AdminUser = targetUser;

        await RevokeAllRefreshTokensAsync(targetUser.Id, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(branch);
    }
}
