using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers.Executors;

public class UpdateCustomerProfileExecutor : CustomerExecutorBase, IUpdateCustomerProfileExecutor
{
    public UpdateCustomerProfileExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ProfileResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(customerId, cancellationToken);
        await EnsureCanViewOrEditCustomerAsync(callerId, callerRole, customer, cancellationToken);

        var profile = await GetOrCreateProfileAsync(customerId, cancellationToken);

        if (request.HairColor is not null)
        {
            profile.HairColor = request.HairColor;
        }

        if (request.Preferences is not null)
        {
            profile.Preferences = request.Preferences;
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToProfileResponse(profile);
    }
}
