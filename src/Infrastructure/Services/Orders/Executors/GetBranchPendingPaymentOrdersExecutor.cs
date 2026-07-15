using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class GetBranchPendingPaymentOrdersExecutor : OrderExecutorBase, IGetBranchPendingPaymentOrdersExecutor
{
    public GetBranchPendingPaymentOrdersExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == branchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        if (callerRole != UserRole.CentralOffice && callerRole != UserRole.BranchAdmin)
        {
            throw new AppException("You are not allowed to list branch pending-payment orders.", AppErrorType.Forbidden);
        }

        await EnsureCanManageBranchAsync(callerId, callerRole, branchId, cancellationToken);

        var orders = await OrdersWithItems()
            .Where(o => o.BranchId == branchId && o.PaymentId == null && !o.Cancelled)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(ToResponse).ToList();
    }
}
