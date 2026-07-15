using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Payments.Executors;

public interface ICreatePaymentExecutor
{
    Task<PaymentResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default);
}
