using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders;

/// <summary>
/// Shared helpers for order executors: mapping, scope checks, and total recalculation.
/// </summary>
public abstract class OrderExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IBranchScopeChecker BranchScopeChecker;

    protected OrderExecutorBase(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        DbContext = dbContext;
        BranchScopeChecker = branchScopeChecker;
    }

    protected static OrderResponse ToResponse(Order order) => new()
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        StaffId = order.StaffId,
        BranchId = order.BranchId,
        TotalPrice = order.TotalPrice,
        Date = order.Date,
        Completed = order.Completed,
        Cancelled = order.Cancelled,
        PaymentId = order.PaymentId,
        Comment = order.Comment,
        Items = order.Items
            .OrderBy(i => i.ServiceName)
            .Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ServiceId = i.ServiceId,
                ServiceName = i.ServiceName,
                ServicePrice = i.ServicePrice
            })
            .ToList(),
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };

    protected static void RecalculateTotal(Order order) =>
        order.TotalPrice = order.Items.Sum(i => i.ServicePrice);

    protected static void EnsureOrderMutable(Order order)
    {
        if (order.Completed)
        {
            throw new AppException("Completed orders cannot be modified.", AppErrorType.Validation);
        }

        if (order.Cancelled)
        {
            throw new AppException("Cancelled orders cannot be modified.", AppErrorType.Validation);
        }
    }

    protected async Task<Order> GetOrderWithItemsOrThrowAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await DbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
        {
            throw new AppException("Order not found.", AppErrorType.NotFound);
        }

        return order;
    }

    protected async Task EnsureCanManageBranchAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null || administeredBranch.Id != branchId)
            {
                throw new AppException("BranchAdmin can only manage orders for their own branch.", AppErrorType.Forbidden);
            }

            return;
        }

        if (callerRole == UserRole.Staff)
        {
            var caller = await DbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == callerId, cancellationToken);
            if (caller?.BranchId is null || caller.BranchId != branchId)
            {
                throw new AppException("Staff can only manage orders for their own branch.", AppErrorType.Forbidden);
            }

            return;
        }

        throw new AppException("You are not allowed to manage orders.", AppErrorType.Forbidden);
    }

    protected async Task EnsureCanViewOrderAsync(
        Guid callerId,
        UserRole callerRole,
        Order order,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);
            return;
        }

        if (callerRole == UserRole.Staff && order.StaffId == callerId)
        {
            return;
        }

        if (callerRole == UserRole.Customer && order.CustomerId == callerId)
        {
            return;
        }

        throw new AppException("You are not allowed to view this order.", AppErrorType.Forbidden);
    }

    protected async Task<(User Customer, User Staff)> ValidateCustomerAndStaffAsync(
        Guid customerId,
        Guid staffId,
        CancellationToken cancellationToken)
    {
        var customer = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == customerId, cancellationToken);
        if (customer is null || customer.Role != UserRole.Customer)
        {
            throw new AppException("Customer not found.", AppErrorType.NotFound);
        }

        var staff = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == staffId, cancellationToken);
        if (staff is null || staff.Role != UserRole.Staff)
        {
            throw new AppException("Staff not found.", AppErrorType.NotFound);
        }

        if (!customer.BranchId.HasValue || !staff.BranchId.HasValue || customer.BranchId != staff.BranchId)
        {
            throw new AppException("Customer and Staff must belong to the same branch.", AppErrorType.Validation);
        }

        return (customer, staff);
    }

    protected IQueryable<Order> OrdersWithItems() =>
        DbContext.Orders.Include(o => o.Items).AsQueryable();
}
