using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class CancelOrderExecutor : OrderExecutorBase, ICancelOrderExecutor
{
    public CancelOrderExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderWithItemsOrThrowAsync(orderId, cancellationToken);
        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);

        if (order.Completed)
        {
            throw new AppException("Completed orders cannot be cancelled.", AppErrorType.Validation);
        }

        order.Cancelled = true;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
