using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Branch management: list/get/create/update, activate/deactivate/freeze,
/// and assign BranchAdmin. Creation is CentralOffice-only; other write actions
/// allow the BranchAdmin of that specific branch.
/// </summary>
[Route("branches")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin")]
public class BranchesController : ApiControllerBase
{
    private readonly IGetBranchesExecutor _getBranchesExecutor;
    private readonly IGetBranchByIdExecutor _getBranchByIdExecutor;
    private readonly ICreateBranchExecutor _createBranchExecutor;
    private readonly IUpdateBranchExecutor _updateBranchExecutor;
    private readonly IDeactivateBranchExecutor _deactivateBranchExecutor;
    private readonly IActivateBranchExecutor _activateBranchExecutor;
    private readonly IFreezeBranchExecutor _freezeBranchExecutor;
    private readonly IAssignBranchAdminExecutor _assignBranchAdminExecutor;

    public BranchesController(
        IGetBranchesExecutor getBranchesExecutor,
        IGetBranchByIdExecutor getBranchByIdExecutor,
        ICreateBranchExecutor createBranchExecutor,
        IUpdateBranchExecutor updateBranchExecutor,
        IDeactivateBranchExecutor deactivateBranchExecutor,
        IActivateBranchExecutor activateBranchExecutor,
        IFreezeBranchExecutor freezeBranchExecutor,
        IAssignBranchAdminExecutor assignBranchAdminExecutor)
    {
        _getBranchesExecutor = getBranchesExecutor;
        _getBranchByIdExecutor = getBranchByIdExecutor;
        _createBranchExecutor = createBranchExecutor;
        _updateBranchExecutor = updateBranchExecutor;
        _deactivateBranchExecutor = deactivateBranchExecutor;
        _activateBranchExecutor = activateBranchExecutor;
        _freezeBranchExecutor = freezeBranchExecutor;
        _assignBranchAdminExecutor = assignBranchAdminExecutor;
    }

    /// <summary>
    /// Lists branches. CentralOffice sees all; BranchAdmin sees only the branch they administer.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BranchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchesExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Returns a specific branch by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchByIdExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Creates a branch and its initial BranchAdmin user. CentralOffice only.
    /// This is the only way to create a BranchAdmin account besides assign-admin.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "CentralOffice")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createBranchExecutor.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Updates Name, Address, and Phone on a branch.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] UpdateBranchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateBranchExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Soft-deactivates a branch (IsActive = false).</summary>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateBranch(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _deactivateBranchExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Activates a branch (IsActive = true).</summary>
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateBranch(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _activateBranchExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Freezes a branch (IsFrozen = true).</summary>
    [HttpPatch("{id:guid}/freeze")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FreezeBranch(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _freezeBranchExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Promotes a Staff user of the branch to BranchAdmin, demoting the previous admin to Staff if any.
    /// </summary>
    [HttpPost("{id:guid}/assign-admin")]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignBranchAdmin(Guid id, [FromBody] AssignBranchAdminRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _assignBranchAdminExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
