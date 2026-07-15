using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers.Executors;

public class UpdateCustomerExecutor : CustomerExecutorBase, IUpdateCustomerExecutor
{
    public UpdateCustomerExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<CustomerResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(customerId, cancellationToken);
        await EnsureCanViewOrEditCustomerAsync(callerId, callerRole, customer, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (normalizedEmail != customer.Email)
            {
                var emailTaken = await DbContext.Users
                    .AnyAsync(u => u.Email == normalizedEmail && u.Id != customerId, cancellationToken);
                if (emailTaken)
                {
                    throw new AppException("Email is already registered.", AppErrorType.Conflict);
                }

                customer.Email = normalizedEmail;
            }
        }

        if (request.Phone is not null)
        {
            customer.Phone = request.Phone;
        }

        if (request.Address is not null)
        {
            customer.Address = request.Address;
        }

        if (request.Birthday.HasValue)
        {
            customer.Birthday = request.Birthday;
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToCustomerResponse(customer);
    }
}
