using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class AddOrderStaffExecutor : OrderExecutorBase, IAddOrderStaffExecutor
{
    public AddOrderStaffExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        AddOrderStaffRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderOrThrowAsync(orderId, cancellationToken);
        EnsureOrderMutable(order);
        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);

        var staff = await ValidateStaffForBranchAsync(request.StaffId, order.BranchId, cancellationToken);
        order.StaffId = staff.Id;

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
