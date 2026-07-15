using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers.Executors;

public class GetCustomersExecutor : CustomerExecutorBase, IGetCustomersExecutor
{
    public GetCustomersExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<CustomerResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.Users.Where(u => u.Role == UserRole.Customer);

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null)
            {
                return Array.Empty<CustomerResponse>();
            }

            query = query.Where(u => u.BranchId == administeredBranch.Id);
        }

        var customers = await query
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return customers.Select(ToCustomerResponse).ToList();
    }
}
