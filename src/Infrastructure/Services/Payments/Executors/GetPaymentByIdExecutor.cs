using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Payments.Executors;

public class GetPaymentByIdExecutor : PaymentExecutorBase, IGetPaymentByIdExecutor
{
    public GetPaymentByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<PaymentResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        var payment = await DbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        if (payment is null)
        {
            throw new AppException("Payment not found.", AppErrorType.NotFound);
        }

        await EnsureCanViewPaymentAsync(callerId, callerRole, payment, cancellationToken);
        return ToResponse(payment);
    }
}
