using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Expense category management for CentralOffice and BranchAdmin (branch-scoped).
/// </summary>
[Route("expense-categories")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin")]
public class ExpenseCategoriesController : ApiControllerBase
{
    private readonly IGetExpenseCategoriesExecutor _getExpenseCategoriesExecutor;
    private readonly IGetExpenseCategoryByIdExecutor _getExpenseCategoryByIdExecutor;
    private readonly ICreateExpenseCategoryExecutor _createExpenseCategoryExecutor;
    private readonly IUpdateExpenseCategoryExecutor _updateExpenseCategoryExecutor;
    private readonly IDeleteExpenseCategoryExecutor _deleteExpenseCategoryExecutor;

    public ExpenseCategoriesController(
        IGetExpenseCategoriesExecutor getExpenseCategoriesExecutor,
        IGetExpenseCategoryByIdExecutor getExpenseCategoryByIdExecutor,
        ICreateExpenseCategoryExecutor createExpenseCategoryExecutor,
        IUpdateExpenseCategoryExecutor updateExpenseCategoryExecutor,
        IDeleteExpenseCategoryExecutor deleteExpenseCategoryExecutor)
    {
        _getExpenseCategoriesExecutor = getExpenseCategoriesExecutor;
        _getExpenseCategoryByIdExecutor = getExpenseCategoryByIdExecutor;
        _createExpenseCategoryExecutor = createExpenseCategoryExecutor;
        _updateExpenseCategoryExecutor = updateExpenseCategoryExecutor;
        _deleteExpenseCategoryExecutor = deleteExpenseCategoryExecutor;
    }

    /// <summary>Lists expense categories. CentralOffice sees all; BranchAdmin sees only their branch.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenseCategories(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getExpenseCategoriesExecutor.ExecuteAsync(
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

    /// <summary>Returns an expense category by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExpenseCategoryById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getExpenseCategoryByIdExecutor.ExecuteAsync(
                GetCurrentUserId(),
                GetCurrentUserRole(),
                id,
                cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Creates an expense category for a branch. BranchAdmin may only create for their own branch.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateExpenseCategory(
        [FromBody] CreateExpenseCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createExpenseCategoryExecutor.ExecuteAsync(
                GetCurrentUserId(),
                GetCurrentUserRole(),
                request,
                cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Updates an expense category name.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateExpenseCategory(
        Guid id,
        [FromBody] UpdateExpenseCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateExpenseCategoryExecutor.ExecuteAsync(
                GetCurrentUserId(),
                GetCurrentUserRole(),
                id,
                request,
                cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Deletes an expense category that has no expenses.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteExpenseCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _deleteExpenseCategoryExecutor.ExecuteAsync(
                GetCurrentUserId(),
                GetCurrentUserRole(),
                id,
                cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
