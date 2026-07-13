using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Auth endpoints: login, logout, token refresh, registration
/// (customer/staff/Central Office bootstrap), password change, and forgot/reset password.
/// </summary>
[Route("auth")]
[Produces("application/json")]
public class AuthController : ApiControllerBase
{
    private readonly ILoginExecutor _loginExecutor;
    private readonly ILogoutExecutor _logoutExecutor;
    private readonly IRefreshTokenExecutor _refreshTokenExecutor;
    private readonly IRegisterCustomerExecutor _registerCustomerExecutor;
    private readonly IRegisterStaffExecutor _registerStaffExecutor;
    private readonly IRegisterCentralOfficeExecutor _registerCentralOfficeExecutor;
    private readonly IChangePasswordExecutor _changePasswordExecutor;
    private readonly IForgotPasswordExecutor _forgotPasswordExecutor;
    private readonly IResetPasswordExecutor _resetPasswordExecutor;

    public AuthController(
        ILoginExecutor loginExecutor,
        ILogoutExecutor logoutExecutor,
        IRefreshTokenExecutor refreshTokenExecutor,
        IRegisterCustomerExecutor registerCustomerExecutor,
        IRegisterStaffExecutor registerStaffExecutor,
        IRegisterCentralOfficeExecutor registerCentralOfficeExecutor,
        IChangePasswordExecutor changePasswordExecutor,
        IForgotPasswordExecutor forgotPasswordExecutor,
        IResetPasswordExecutor resetPasswordExecutor)
    {
        _loginExecutor = loginExecutor;
        _logoutExecutor = logoutExecutor;
        _refreshTokenExecutor = refreshTokenExecutor;
        _registerCustomerExecutor = registerCustomerExecutor;
        _registerStaffExecutor = registerStaffExecutor;
        _registerCentralOfficeExecutor = registerCentralOfficeExecutor;
        _changePasswordExecutor = changePasswordExecutor;
        _forgotPasswordExecutor = forgotPasswordExecutor;
        _resetPasswordExecutor = resetPasswordExecutor;
    }

    /// <summary>Authenticates a user with email and password.</summary>
    /// <remarks>Returns a short-lived access token and a long-lived refresh token.</remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _loginExecutor.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Revokes the current user's refresh tokens, ending their session(s).</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _logoutExecutor.ExecuteAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Exchanges a valid refresh token for a new access token, rotating the refresh token.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthTokensResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _refreshTokenExecutor.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Registers a new customer account. Publicly accessible.</summary>
    [HttpPost("register-customer")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _registerCustomerExecutor.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Registers a new Staff account for a branch. Requires an authenticated
    /// CentralOffice or BranchAdmin user; a BranchAdmin may only register staff
    /// for the branch they administer.
    /// </summary>
    [HttpPost("register-staff")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _registerStaffExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Bootstraps the single Central Office account. Publicly accessible, but only
    /// succeeds while no Central Office user exists yet; returns 400 Bad Request afterwards.
    /// </summary>
    [HttpPost("register-central-office")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterCentralOffice([FromBody] RegisterCentralOfficeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _registerCentralOfficeExecutor.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Changes the authenticated user's password and revokes all existing refresh tokens.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _changePasswordExecutor.ExecuteAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Starts the password reset flow by emailing a reset link when the email is
    /// registered. Always returns a generic success message to avoid revealing
    /// whether an account exists.
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await _forgotPasswordExecutor.ExecuteAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>Completes the password reset flow using the token emailed to the user.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _resetPasswordExecutor.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
