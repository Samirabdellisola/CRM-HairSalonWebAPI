using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches.Executors;

public class GetBranchesExecutor : BranchExecutorBase, IGetBranchesExecutor
{
    public GetBranchesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<BranchResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null)
            {
                return Array.Empty<BranchResponse>();
            }

            return new[] { ToResponse(administeredBranch) };
        }

        var branches = await DbContext.Branches
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

        return branches.Select(ToResponse).ToList();
    }
}
