using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Common.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>Revokes the caller's active refresh tokens. The provided token must belong to the authenticated user.</summary>
public interface ILogoutExecutor
{
    Task<GenericResponse> ExecuteAsync(Guid userId, RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
