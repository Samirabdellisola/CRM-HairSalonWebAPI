using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Payments.Executors;

public class GetOrderPaymentsExecutor : PaymentExecutorBase, IGetOrderPaymentsExecutor
{
    public GetOrderPaymentsExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<PaymentResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await DbContext.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
        {
            throw new AppException("Order not found.", AppErrorType.NotFound);
        }

        await EnsureCanViewOrderAsync(callerId, callerRole, order, cancellationToken);

        var payments = await DbContext.Payments
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.Date)
            .ToListAsync(cancellationToken);

        return payments.Select(ToResponse).ToList();
    }
}
