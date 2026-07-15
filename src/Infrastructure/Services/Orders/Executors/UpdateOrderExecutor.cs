using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class UpdateOrderExecutor : OrderExecutorBase, IUpdateOrderExecutor
{
    public UpdateOrderExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderWithItemsOrThrowAsync(orderId, cancellationToken);
        EnsureOrderMutable(order);
        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);

        var (customer, staff) = await ValidateCustomerAndStaffAsync(request.CustomerId, request.StaffId, cancellationToken);
        if (staff.BranchId != order.BranchId || customer.BranchId != order.BranchId)
        {
            throw new AppException("Customer and Staff must belong to the order's branch.", AppErrorType.Validation);
        }

        order.CustomerId = customer.Id;
        order.StaffId = staff.Id;
        order.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
