using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Customers.Executors;

public interface IUpdateCustomerProfileExecutor
{
    Task<ProfileResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default);
}
