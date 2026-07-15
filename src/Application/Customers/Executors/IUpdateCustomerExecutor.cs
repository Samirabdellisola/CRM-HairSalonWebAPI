using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Customers.Executors;

public interface IUpdateCustomerExecutor
{
    Task<CustomerResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default);
}
