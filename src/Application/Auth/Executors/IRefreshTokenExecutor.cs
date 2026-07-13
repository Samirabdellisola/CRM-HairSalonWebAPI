using SalonCRM.Application.Auth.DTOs;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>Exchanges a valid refresh token for a new access token, rotating the refresh token.</summary>
public interface IRefreshTokenExecutor
{
    Task<AuthTokensResponse> ExecuteAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
