using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class GetCustomerOrdersExecutor : OrderExecutorBase, IGetCustomerOrdersExecutor
{
    public GetCustomerOrdersExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == customerId, cancellationToken);
        if (customer is null || customer.Role != UserRole.Customer)
        {
            throw new AppException("Customer not found.", AppErrorType.NotFound);
        }

        if (callerRole == UserRole.Customer)
        {
            if (callerId != customerId)
            {
                throw new AppException("Customers can only view their own orders.", AppErrorType.Forbidden);
            }
        }
        else if (callerRole == UserRole.BranchAdmin)
        {
            if (!customer.BranchId.HasValue)
            {
                throw new AppException("BranchAdmin can only view customers within their own branch.", AppErrorType.Forbidden);
            }

            await EnsureCanManageBranchAsync(callerId, callerRole, customer.BranchId.Value, cancellationToken);
        }
        else if (callerRole != UserRole.CentralOffice)
        {
            throw new AppException("You are not allowed to list customer orders.", AppErrorType.Forbidden);
        }

        var orders = await OrdersWithItems()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(ToResponse).ToList();
    }
}
