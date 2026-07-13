using Microsoft.EntityFrameworkCore;
using SalonCRM.Domain.Entities;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure.Services.Common;

public class BranchScopeChecker : IBranchScopeChecker
{
    private readonly AppDbContext _dbContext;

    public BranchScopeChecker(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Branch?> GetAdministeredBranchAsync(Guid branchAdminUserId, CancellationToken cancellationToken = default) =>
        _dbContext.Branches.FirstOrDefaultAsync(b => b.AdminId == branchAdminUserId, cancellationToken);
}
