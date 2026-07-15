using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class CreateOrderExecutor : OrderExecutorBase, ICreateOrderExecutor
{
    public CreateOrderExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var (customer, staff) = await ValidateCustomerAndStaffAsync(request.CustomerId, request.StaffId, cancellationToken);
        var branchId = staff.BranchId!.Value;

        await EnsureCanManageBranchAsync(callerId, callerRole, branchId, cancellationToken);

        var order = new Order
        {
            CustomerId = customer.Id,
            StaffId = staff.Id,
            BranchId = branchId,
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            Completed = false,
            Cancelled = false
        };

        if (request.ServiceIds is { Count: > 0 })
        {
            var distinctServiceIds = request.ServiceIds.Distinct().ToList();
            var services = await DbContext.Services
                .Where(s => distinctServiceIds.Contains(s.Id) && s.BranchId == branchId)
                .ToListAsync(cancellationToken);

            if (services.Count != distinctServiceIds.Count)
            {
                throw new AppException("One or more services were not found for this branch.", AppErrorType.NotFound);
            }

            foreach (var service in services)
            {
                order.Items.Add(new OrderItem
                {
                    ServiceId = service.Id,
                    ServiceName = service.Name,
                    ServicePrice = service.Price
                });
            }
        }

        RecalculateTotal(order);

        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
