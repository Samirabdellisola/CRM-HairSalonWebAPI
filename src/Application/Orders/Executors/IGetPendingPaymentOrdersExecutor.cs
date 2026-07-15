using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Orders.Executors;

public interface IGetPendingPaymentOrdersExecutor
{
    Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
