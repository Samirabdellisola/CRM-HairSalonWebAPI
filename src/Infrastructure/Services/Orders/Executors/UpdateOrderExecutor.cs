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
        var order = await GetOrderOrThrowAsync(orderId, cancellationToken);
        EnsureOrderMutable(order);
        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);

        var customer = await ValidateCustomerAsync(request.CustomerId, cancellationToken);
        if (customer.BranchId != order.BranchId)
        {
            throw new AppException("Customer must belong to the order's branch.", AppErrorType.Validation);
        }

        if (request.StaffId.HasValue)
        {
            EnsureCanTargetOrderStaff(callerId, callerRole, request.StaffId.Value);
        }
        else if (callerRole == UserRole.Staff && order.StaffId.HasValue && order.StaffId.Value != callerId)
        {
            throw new AppException("Staff can only add or remove themselves on an order.", AppErrorType.Forbidden);
        }

        var staff = await ValidateOptionalStaffForBranchAsync(request.StaffId, order.BranchId, cancellationToken);
        var service = await ValidateServiceForBranchAsync(request.ServiceId, order.BranchId, cancellationToken);

        order.CustomerId = customer.Id;
        order.StaffId = staff?.Id;
        order.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
        ApplyServiceSnapshot(order, service);

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
