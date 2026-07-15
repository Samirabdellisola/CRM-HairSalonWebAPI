using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Orders.Executors;

public interface IRemoveOrderServiceExecutor
{
    Task<OrderResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid orderId,
        RemoveOrderServiceRequest request,
        CancellationToken cancellationToken = default);
}
