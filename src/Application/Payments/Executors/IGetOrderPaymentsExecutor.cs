using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Payments.Executors;

public interface IGetOrderPaymentsExecutor
{
    Task<IReadOnlyList<PaymentResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        CancellationToken cancellationToken = default);
}
