using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;
using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Application.Staff.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Branch management: list/get/create/update, activate/deactivate/freeze,
/// and assign BranchAdmin. List and get-by-id are public; creation is
/// CentralOffice-only; other write actions allow the BranchAdmin of that branch.
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
    private readonly IGetBranchStaffExecutor _getBranchStaffExecutor;
    private readonly IGetBranchCustomersExecutor _getBranchCustomersExecutor;
    private readonly IGetBranchOrdersExecutor _getBranchOrdersExecutor;
    private readonly IGetBranchPendingPaymentOrdersExecutor _getBranchPendingPaymentOrdersExecutor;
    private readonly IGetBranchPaymentsExecutor _getBranchPaymentsExecutor;
    private readonly IGetBranchExpensesExecutor _getBranchExpensesExecutor;

    public BranchesController(
        IGetBranchesExecutor getBranchesExecutor,
        IGetBranchByIdExecutor getBranchByIdExecutor,
        ICreateBranchExecutor createBranchExecutor,
        IUpdateBranchExecutor updateBranchExecutor,
        IDeactivateBranchExecutor deactivateBranchExecutor,
        IActivateBranchExecutor activateBranchExecutor,
        IFreezeBranchExecutor freezeBranchExecutor,
        IAssignBranchAdminExecutor assignBranchAdminExecutor,
        IGetBranchStaffExecutor getBranchStaffExecutor,
        IGetBranchCustomersExecutor getBranchCustomersExecutor,
        IGetBranchOrdersExecutor getBranchOrdersExecutor,
        IGetBranchPendingPaymentOrdersExecutor getBranchPendingPaymentOrdersExecutor,
        IGetBranchPaymentsExecutor getBranchPaymentsExecutor,
        IGetBranchExpensesExecutor getBranchExpensesExecutor)
    {
        _getBranchesExecutor = getBranchesExecutor;
        _getBranchByIdExecutor = getBranchByIdExecutor;
        _createBranchExecutor = createBranchExecutor;
        _updateBranchExecutor = updateBranchExecutor;
        _deactivateBranchExecutor = deactivateBranchExecutor;
        _activateBranchExecutor = activateBranchExecutor;
        _freezeBranchExecutor = freezeBranchExecutor;
        _assignBranchAdminExecutor = assignBranchAdminExecutor;
        _getBranchStaffExecutor = getBranchStaffExecutor;
        _getBranchCustomersExecutor = getBranchCustomersExecutor;
        _getBranchOrdersExecutor = getBranchOrdersExecutor;
        _getBranchPendingPaymentOrdersExecutor = getBranchPendingPaymentOrdersExecutor;
        _getBranchPaymentsExecutor = getBranchPaymentsExecutor;
        _getBranchExpensesExecutor = getBranchExpensesExecutor;
    }

    /// <summary>Lists all branches. No authentication required.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<BranchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchesExecutor.ExecuteAsync(cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Returns a specific branch by id. No authentication required.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BranchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchByIdExecutor.ExecuteAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists Staff users belonging to a branch. CentralOffice or BranchAdmin of that branch.
    /// </summary>
    [HttpGet("{branchId:guid}/staff")]
    [ProducesResponseType(typeof(IReadOnlyList<StaffResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchStaff(Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchStaffExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), branchId, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists Customer users belonging to a branch. CentralOffice or BranchAdmin of that branch.
    /// </summary>
    [HttpGet("{branchId:guid}/customers")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchCustomers(Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchCustomersExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), branchId, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists orders for a branch. CentralOffice or BranchAdmin of that branch.
    /// </summary>
    [HttpGet("{branchId:guid}/orders")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchOrders(Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchOrdersExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), branchId, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists orders for a branch that have no PaymentId.
    /// </summary>
    [HttpGet("{branchId:guid}/orders/pending-payment")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchPendingPaymentOrders(Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchPendingPaymentOrdersExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), branchId, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists payments for a branch. CentralOffice or BranchAdmin of that branch.
    /// </summary>
    [HttpGet("{branchId:guid}/payments")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchPayments(Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchPaymentsExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), branchId, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists expenses for a branch. CentralOffice or BranchAdmin of that branch.
    /// </summary>
    [HttpGet("{branchId:guid}/expenses")]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchExpenses(Guid branchId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getBranchExpensesExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), branchId, cancellationToken);
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
