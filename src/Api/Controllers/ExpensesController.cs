using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Expense management for CentralOffice and BranchAdmin (branch-scoped).
/// </summary>
[Route("expenses")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin")]
public class ExpensesController : ApiControllerBase
{
    private readonly IGetExpensesExecutor _getExpensesExecutor;
    private readonly IGetExpenseByIdExecutor _getExpenseByIdExecutor;
    private readonly IGetExpenseCategoriesForExpensesExecutor _getExpenseCategoriesForExpensesExecutor;
    private readonly IGetExpensesByRangeExecutor _getExpensesByRangeExecutor;
    private readonly ICreateExpenseExecutor _createExpenseExecutor;
    private readonly IUpdateExpenseExecutor _updateExpenseExecutor;
    private readonly IDeleteExpenseExecutor _deleteExpenseExecutor;

    public ExpensesController(
        IGetExpensesExecutor getExpensesExecutor,
        IGetExpenseByIdExecutor getExpenseByIdExecutor,
        IGetExpenseCategoriesForExpensesExecutor getExpenseCategoriesForExpensesExecutor,
        IGetExpensesByRangeExecutor getExpensesByRangeExecutor,
        ICreateExpenseExecutor createExpenseExecutor,
        IUpdateExpenseExecutor updateExpenseExecutor,
        IDeleteExpenseExecutor deleteExpenseExecutor)
    {
        _getExpensesExecutor = getExpensesExecutor;
        _getExpenseByIdExecutor = getExpenseByIdExecutor;
        _getExpenseCategoriesForExpensesExecutor = getExpenseCategoriesForExpensesExecutor;
        _getExpensesByRangeExecutor = getExpensesByRangeExecutor;
        _createExpenseExecutor = createExpenseExecutor;
        _updateExpenseExecutor = updateExpenseExecutor;
        _deleteExpenseExecutor = deleteExpenseExecutor;
    }

    /// <summary>Lists expenses. CentralOffice sees all; BranchAdmin sees only their branch.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getExpensesExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists expense categories available for creating expenses.
    /// CentralOffice sees all; BranchAdmin sees only their branch.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenseCategories(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getExpenseCategoriesForExpensesExecutor.ExecuteAsync(
                GetCurrentUserId(),
                GetCurrentUserRole(),
                cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Lists expenses whose Date is within the inclusive from/to range (UTC).</summary>
    [HttpGet("by-range")]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExpensesByRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getExpensesByRangeExecutor.ExecuteAsync(
                GetCurrentUserId(),
                GetCurrentUserRole(),
                from,
                to,
                cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Returns an expense by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExpenseById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getExpenseByIdExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Creates an expense.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createExpenseExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Updates an expense.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExpense(
        Guid id,
        [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateExpenseExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Deletes an expense.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _deleteExpenseExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
