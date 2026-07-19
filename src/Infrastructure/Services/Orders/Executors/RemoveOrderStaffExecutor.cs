using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class RemoveOrderStaffExecutor : OrderExecutorBase, IRemoveOrderStaffExecutor
{
    public RemoveOrderStaffExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        RemoveOrderStaffRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderOrThrowAsync(orderId, cancellationToken);
        EnsureOrderMutable(order);
        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);
        EnsureCanTargetOrderStaff(callerId, callerRole, request.StaffId);

        if (!order.StaffId.HasValue)
        {
            throw new AppException("This order has no staff assigned.", AppErrorType.Validation);
        }

        if (order.StaffId.Value != request.StaffId)
        {
            throw new AppException("The specified staff is not assigned to this order.", AppErrorType.Validation);
        }

        order.StaffId = null;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
