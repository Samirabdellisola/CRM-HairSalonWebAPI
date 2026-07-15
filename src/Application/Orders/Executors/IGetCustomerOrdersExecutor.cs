using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Orders.Executors;

public interface IGetCustomerOrdersExecutor
{
    Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        CancellationToken cancellationToken = default);
}
