using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class AddOrderServiceExecutor : OrderExecutorBase, IAddOrderServiceExecutor
{
    public AddOrderServiceExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        AddOrderServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderWithItemsOrThrowAsync(orderId, cancellationToken);
        EnsureOrderMutable(order);
        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);

        if (order.Items.Any(i => i.ServiceId == request.ServiceId))
        {
            throw new AppException("This service is already on the order.", AppErrorType.Conflict);
        }

        var service = await DbContext.Services.FirstOrDefaultAsync(
            s => s.Id == request.ServiceId && s.BranchId == order.BranchId,
            cancellationToken);
        if (service is null)
        {
            throw new AppException("Service not found for this branch.", AppErrorType.NotFound);
        }

        order.Items.Add(new OrderItem
        {
            ServiceId = service.Id,
            ServiceName = service.Name,
            ServicePrice = service.Price
        });

        RecalculateTotal(order);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
