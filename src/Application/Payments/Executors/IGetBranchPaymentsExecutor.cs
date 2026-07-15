using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Payments.Executors;

public interface IGetBranchPaymentsExecutor
{
    Task<IReadOnlyList<PaymentResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default);
}
