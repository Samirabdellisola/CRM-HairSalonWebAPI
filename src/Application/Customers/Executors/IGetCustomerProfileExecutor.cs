using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Customers.Executors;

public interface IGetCustomerProfileExecutor
{
    Task<ProfileResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        CancellationToken cancellationToken = default);
}
