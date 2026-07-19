using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Payments.Executors;

public class CreatePaymentExecutor : PaymentExecutorBase, ICreatePaymentExecutor
{
    public CreatePaymentExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<PaymentResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await DbContext.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order is null)
        {
            throw new AppException("Order not found.", AppErrorType.NotFound);
        }

        if (order.Cancelled)
        {
            throw new AppException("Cannot create a payment for a cancelled order.", AppErrorType.Validation);
        }

        if (order.PaymentId.HasValue)
        {
            throw new AppException("This order already has a payment.", AppErrorType.Conflict);
        }

        if (!order.StaffId.HasValue)
        {
            throw new AppException("Order must have a staff member before creating a payment.", AppErrorType.Validation);
        }

        await EnsureCanManageBranchAsync(callerId, callerRole, order.BranchId, cancellationToken);

        if (!Enum.IsDefined(request.PaymentMethod))
        {
            throw new AppException("Invalid payment method.", AppErrorType.Validation);
        }

        var payment = new Payment
        {
            PaymentMethod = request.PaymentMethod,
            CustomerId = order.CustomerId,
            StaffId = order.StaffId.Value,
            BranchId = order.BranchId,
            OrderId = order.Id,
            Date = request.Date?.ToUniversalTime() ?? DateTime.UtcNow
        };

        DbContext.Payments.Add(payment);
        await DbContext.SaveChangesAsync(cancellationToken);

        order.PaymentId = payment.Id;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(payment);
    }
}
