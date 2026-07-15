using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers.Executors;

public class GetBranchCustomersExecutor : CustomerExecutorBase, IGetBranchCustomersExecutor
{
    public GetBranchCustomersExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<CustomerResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == branchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        await EnsureCanManageCustomerAsync(callerId, callerRole, branchId, cancellationToken);

        var customers = await DbContext.Users
            .Where(u => u.Role == UserRole.Customer && u.BranchId == branchId)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return customers.Select(ToCustomerResponse).ToList();
    }
}
