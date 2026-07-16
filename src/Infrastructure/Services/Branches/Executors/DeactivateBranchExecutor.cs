using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches.Executors;

public class DeactivateBranchExecutor : BranchExecutorBase, IDeactivateBranchExecutor
{
    public DeactivateBranchExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanManageBranchAsync(callerId, callerRole, branchId, cancellationToken);

        var branch = await BranchesWithAdmin.FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken);
        if (branch is null)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        branch.IsActive = false;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(branch);
    }
}
