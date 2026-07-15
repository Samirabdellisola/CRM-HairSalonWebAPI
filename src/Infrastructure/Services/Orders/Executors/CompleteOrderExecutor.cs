using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class CompleteOrderExecutor : OrderExecutorBase, ICompleteOrderExecutor
{
    public CompleteOrderExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
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

        if (order.Cancelled)
        {
            throw new AppException("Cancelled orders cannot be completed.", AppErrorType.Validation);
        }

        order.Completed = true;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
