using SalonCRM.Application.Auth.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>Authenticates a user with email and password. Throws AppException on failure.</summary>
public interface ILoginExecutor
{
    Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
