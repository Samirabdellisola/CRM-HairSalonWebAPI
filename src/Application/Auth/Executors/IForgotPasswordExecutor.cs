using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Common.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>
/// Always returns a generic success response regardless of whether the email exists,
/// to avoid leaking account existence.
/// </summary>
public interface IForgotPasswordExecutor
{
    Task<GenericResponse> ExecuteAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
}
