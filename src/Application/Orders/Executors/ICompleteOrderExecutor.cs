using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Orders.Executors;

public interface ICompleteOrderExecutor
{
    Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        CancellationToken cancellationToken = default);
}
