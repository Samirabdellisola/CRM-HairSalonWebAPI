using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Users.DTOs;
using SalonCRM.Application.Users.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// User management endpoints: edit user details, change a user's role, and
/// delete a user. Restricted to CentralOffice and BranchAdmin; a BranchAdmin
/// may only manage Staff/Customer users within the branch they administer.
/// </summary>
[Route("users")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin")]
public class UsersController : ApiControllerBase
{
    private readonly IUpdateUserExecutor _updateUserExecutor;
    private readonly IUpdateUserRoleExecutor _updateUserRoleExecutor;
    private readonly IDeleteUserExecutor _deleteUserExecutor;

    public UsersController(
        IUpdateUserExecutor updateUserExecutor,
        IUpdateUserRoleExecutor updateUserRoleExecutor,
        IDeleteUserExecutor deleteUserExecutor)
    {
        _updateUserExecutor = updateUserExecutor;
        _updateUserRoleExecutor = updateUserRoleExecutor;
        _deleteUserExecutor = deleteUserExecutor;
    }

    /// <summary>Updates Email/Phone/Address on a user.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateUserExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Changes a user's role.</summary>
    [HttpPut("{id:guid}/role")]
    [ProducesResponseType(typeof(UserDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateUserRoleExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Deletes a user.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _deleteUserExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
