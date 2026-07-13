using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Common.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>Changes the authenticated user's password and revokes all existing refresh tokens.</summary>
public interface IChangePasswordExecutor
{
    Task<GenericResponse> ExecuteAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
