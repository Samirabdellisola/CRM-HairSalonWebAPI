using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Orders.Executors;

public interface IGetBranchOrdersExecutor
{
    Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default);
}
