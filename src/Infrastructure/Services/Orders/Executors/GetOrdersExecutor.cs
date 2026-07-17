using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class GetOrdersExecutor : OrderExecutorBase, IGetOrdersExecutor
{
    public GetOrdersExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = DbContext.Orders;

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null)
            {
                return Array.Empty<OrderResponse>();
            }

            query = query.Where(o => o.BranchId == administeredBranch.Id);
        }
        else if (callerRole == UserRole.Staff)
        {
            query = query.Where(o => o.StaffId == callerId);
        }
        else if (callerRole == UserRole.Customer)
        {
            query = query.Where(o => o.CustomerId == callerId);
        }
        else if (callerRole != UserRole.CentralOffice)
        {
            return Array.Empty<OrderResponse>();
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(ToResponse).ToList();
    }
}
