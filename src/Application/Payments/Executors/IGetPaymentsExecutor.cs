using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Payments.Executors;

public interface IGetPaymentsExecutor
{
    Task<IReadOnlyList<PaymentResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
