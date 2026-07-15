using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Customers.Executors;

public interface IGetSpecialDatesExecutor
{
    Task<IReadOnlyList<CustomerResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
