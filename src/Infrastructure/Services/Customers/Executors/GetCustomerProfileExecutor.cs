using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers.Executors;

public class GetCustomerProfileExecutor : CustomerExecutorBase, IGetCustomerProfileExecutor
{
    public GetCustomerProfileExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ProfileResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(customerId, cancellationToken);
        await EnsureCanViewOrEditCustomerAsync(callerId, callerRole, customer, cancellationToken);

        var profile = await GetOrCreateProfileAsync(customerId, cancellationToken);
        return ToProfileResponse(profile);
    }
}
