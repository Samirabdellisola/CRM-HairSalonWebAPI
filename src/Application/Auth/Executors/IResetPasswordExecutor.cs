using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Common.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>Completes the password reset flow using the token emailed to the user.</summary>
public interface IResetPasswordExecutor
{
    Task<GenericResponse> ExecuteAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}
