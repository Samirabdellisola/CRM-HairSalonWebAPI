using SalonCRM.Application.Auth.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>Registers a new Customer account. Publicly accessible.</summary>
public interface IRegisterCustomerExecutor
{
    Task<RegisterUserResponse> ExecuteAsync(RegisterCustomerRequest request, CancellationToken cancellationToken = default);
}
