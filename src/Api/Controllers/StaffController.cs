using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Application.Staff.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Staff management: list/get/update Staff users, and activate/deactivate/freeze.
/// Status changes are restricted to CentralOffice and BranchAdmin of the staff's branch.
/// </summary>
[Route("staff")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
public class StaffController : ApiControllerBase
{
    private readonly IGetStaffListExecutor _getStaffListExecutor;
    private readonly IGetStaffByIdExecutor _getStaffByIdExecutor;
    private readonly IUpdateStaffExecutor _updateStaffExecutor;
    private readonly IActivateStaffExecutor _activateStaffExecutor;
    private readonly IDeactivateStaffExecutor _deactivateStaffExecutor;
    private readonly IFreezeStaffExecutor _freezeStaffExecutor;
    private readonly IGetStaffOrdersExecutor _getStaffOrdersExecutor;

    public StaffController(
        IGetStaffListExecutor getStaffListExecutor,
        IGetStaffByIdExecutor getStaffByIdExecutor,
        IUpdateStaffExecutor updateStaffExecutor,
        IActivateStaffExecutor activateStaffExecutor,
        IDeactivateStaffExecutor deactivateStaffExecutor,
        IFreezeStaffExecutor freezeStaffExecutor,
        IGetStaffOrdersExecutor getStaffOrdersExecutor)
    {
        _getStaffListExecutor = getStaffListExecutor;
        _getStaffByIdExecutor = getStaffByIdExecutor;
        _updateStaffExecutor = updateStaffExecutor;
        _activateStaffExecutor = activateStaffExecutor;
        _deactivateStaffExecutor = deactivateStaffExecutor;
        _freezeStaffExecutor = freezeStaffExecutor;
        _getStaffOrdersExecutor = getStaffOrdersExecutor;
    }

    /// <summary>
    /// Lists Staff users. CentralOffice sees all; BranchAdmin sees only staff of their branch.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(IReadOnlyList<StaffResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStaff(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getStaffListExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Returns a Staff user by id. CentralOffice, BranchAdmin of that staff's branch, or the staff themselves.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getStaffByIdExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists orders for a staff member. CentralOffice, BranchAdmin of their branch, or the staff themselves.
    /// </summary>
    [HttpGet("{staffId:guid}/orders")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffOrders(Guid staffId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getStaffOrdersExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), staffId, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Updates Email/Phone/Address on a Staff user. Does not change password or role.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateStaffExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Activates a Staff user (IsActive = true).</summary>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateStaff(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _activateStaffExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Deactivates a Staff user (IsActive = false).</summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateStaff(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _deactivateStaffExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Freezes a Staff user (IsFrozen = true, e.g. contract ended).</summary>
    [HttpPatch("{id:guid}/freeze")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FreezeStaff(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _freezeStaffExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
