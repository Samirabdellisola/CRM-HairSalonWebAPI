using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Orders.Executors;

public interface IAddOrderServiceExecutor
{
    Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        AddOrderServiceRequest request,
        CancellationToken cancellationToken = default);
}
