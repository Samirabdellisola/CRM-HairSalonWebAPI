using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches.Executors;

public class GetBranchesExecutor : BranchExecutorBase, IGetBranchesExecutor
{
    public GetBranchesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<BranchResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var branches = await BranchesWithAdmin
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

        return branches.Select(ToResponse).ToList();
    }
}
