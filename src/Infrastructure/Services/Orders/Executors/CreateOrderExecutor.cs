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
        var customer = await ValidateCustomerAsync(request.CustomerId, cancellationToken);
        var branchId = customer.BranchId!.Value;

        await EnsureCanManageBranchAsync(callerId, callerRole, branchId, cancellationToken);

        if (request.StaffId.HasValue)
        {
            EnsureCanTargetOrderStaff(callerId, callerRole, request.StaffId.Value);
        }

        var staff = await ValidateOptionalStaffForBranchAsync(request.StaffId, branchId, cancellationToken);
        var service = await ValidateServiceForBranchAsync(request.ServiceId, branchId, cancellationToken);

        var order = new Order
        {
            CustomerId = customer.Id,
            StaffId = staff?.Id,
            BranchId = branchId,
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            Date = request.Date?.ToUniversalTime() ?? DateTime.UtcNow,
            Completed = false,
            Cancelled = false
        };

        ApplyServiceSnapshot(order, service);

        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(order);
    }
}
