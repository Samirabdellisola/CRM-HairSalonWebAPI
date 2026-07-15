using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers.Executors;

public class GetCustomerByIdExecutor : CustomerExecutorBase, IGetCustomerByIdExecutor
{
    public GetCustomerByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<CustomerResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(customerId, cancellationToken);
        await EnsureCanViewOrEditCustomerAsync(callerId, callerRole, customer, cancellationToken);
        return ToCustomerResponse(customer);
    }
}
