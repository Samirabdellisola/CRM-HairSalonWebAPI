using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class RemoveOrderServiceExecutor : OrderExecutorBase, IRemoveOrderServiceExecutor
{
    public RemoveOrderServiceExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        RemoveOrderServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderWithItemsOrThrowAsync(orderId, cancellationToken);
        EnsureOrderMutable(order);
        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);

        var item = order.Items.FirstOrDefault(i => i.ServiceId == request.ServiceId);
        if (item is null)
        {
            throw new AppException("Service is not on this order.", AppErrorType.NotFound);
        }

        order.Items.Remove(item);
        DbContext.OrderItems.Remove(item);

        RecalculateTotal(order);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
