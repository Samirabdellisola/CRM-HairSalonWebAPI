using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Shared helpers for reading the authenticated caller's identity/role from
/// claims and mapping AppException to the right HTTP status code.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.Parse(subject!);
    }

    protected UserRole GetCurrentUserRole()
    {
        var role = User.FindFirstValue("role");
        return Enum.Parse<UserRole>(role!);
    }

    protected ObjectResult HandleAppException(AppException ex)
    {
        var statusCode = ex.ErrorType switch
        {
            AppErrorType.Validation => StatusCodes.Status400BadRequest,
            AppErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            AppErrorType.Forbidden => StatusCodes.Status403Forbidden,
            AppErrorType.NotFound => StatusCodes.Status404NotFound,
            AppErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(statusCode, new GenericResponse { Success = false, Message = ex.Message });
    }
}
