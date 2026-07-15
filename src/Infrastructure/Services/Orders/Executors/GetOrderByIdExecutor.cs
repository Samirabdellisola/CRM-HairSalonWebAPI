using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class GetOrderByIdExecutor : OrderExecutorBase, IGetOrderByIdExecutor
{
    public GetOrderByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
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
        await EnsureCanViewOrderAsync(callerId, callerRole, order, cancellationToken);
        return ToResponse(order);
    }
}
