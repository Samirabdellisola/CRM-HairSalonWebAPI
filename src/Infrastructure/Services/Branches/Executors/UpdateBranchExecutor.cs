using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches.Executors;

public class UpdateBranchExecutor : BranchExecutorBase, IUpdateBranchExecutor
{
    public UpdateBranchExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        UpdateBranchRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanManageBranchAsync(callerId, callerRole, branchId, cancellationToken);

        var branch = await BranchesWithAdmin.FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken);
        if (branch is null)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        var name = request.Name.Trim();
        var nameTaken = await DbContext.Branches.AnyAsync(b => b.Name == name && b.Id != branchId, cancellationToken);
        if (nameTaken)
        {
            throw new AppException("A branch with this name already exists.", AppErrorType.Conflict);
        }

        branch.Name = name;
        branch.Address = request.Address.Trim();
        branch.Phone = request.Phone.Trim();

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(branch);
    }
}
