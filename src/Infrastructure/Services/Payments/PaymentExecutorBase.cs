using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Payments;

/// <summary>
/// Shared helpers for payment executors: mapping and authorization scope checks.
/// </summary>
public abstract class PaymentExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IBranchScopeChecker BranchScopeChecker;

    protected PaymentExecutorBase(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        DbContext = dbContext;
        BranchScopeChecker = branchScopeChecker;
    }

    protected static PaymentResponse ToResponse(Payment payment) => new()
    {
        Id = payment.Id,
        PaymentMethod = payment.PaymentMethod,
        CustomerId = payment.CustomerId,
        StaffId = payment.StaffId,
        BranchId = payment.BranchId,
        OrderId = payment.OrderId,
        Date = payment.Date,
        CreatedAt = payment.CreatedAt,
        UpdatedAt = payment.UpdatedAt
    };

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
                throw new AppException("BranchAdmin can only manage payments for their own branch.", AppErrorType.Forbidden);
            }

            return;
        }

        if (callerRole == UserRole.Staff)
        {
            var caller = await DbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == callerId, cancellationToken);
            if (caller?.BranchId is null || caller.BranchId != branchId)
            {
                throw new AppException("Staff can only manage payments for their own branch.", AppErrorType.Forbidden);
            }

            return;
        }

        throw new AppException("You are not allowed to manage payments.", AppErrorType.Forbidden);
    }

    protected async Task EnsureCanViewPaymentAsync(
        Guid callerId,
        UserRole callerRole,
        Payment payment,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            await EnsureCanManageBranchAsync(callerId, callerRole, payment.BranchId, cancellationToken);
            return;
        }

        if (callerRole == UserRole.Staff && payment.StaffId == callerId)
        {
            return;
        }

        if (callerRole == UserRole.Customer && payment.CustomerId == callerId)
        {
            return;
        }

        throw new AppException("You are not allowed to view this payment.", AppErrorType.Forbidden);
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

        throw new AppException("You are not allowed to view this order's payments.", AppErrorType.Forbidden);
    }
}
