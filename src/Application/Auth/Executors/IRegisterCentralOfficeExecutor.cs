using SalonCRM.Application.Auth.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>
/// Creates the single CentralOffice account. Succeeds only if no CentralOffice
/// user exists yet; otherwise throws AppException with AppErrorType.Validation.
/// </summary>
public interface IRegisterCentralOfficeExecutor
{
    Task<RegisterUserResponse> ExecuteAsync(RegisterCentralOfficeRequest request, CancellationToken cancellationToken = default);
}
