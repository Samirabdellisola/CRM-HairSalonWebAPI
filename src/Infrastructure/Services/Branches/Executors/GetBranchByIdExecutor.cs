using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches.Executors;

public class GetBranchByIdExecutor : BranchExecutorBase, IGetBranchByIdExecutor
{
    public GetBranchByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<BranchResponse> ExecuteAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        var branch = await BranchesWithAdmin.FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken);
        if (branch is null)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        return ToResponse(branch);
    }
}
