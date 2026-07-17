using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Service category catalog CRUD. Writes are restricted to CentralOffice and the
/// BranchAdmin of the category's branch (same access model as services).
/// </summary>
[Route("service-categories")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin,Staff,Customer")]
public class ServiceCategoriesController : ApiControllerBase
{
    private readonly IGetServiceCategoriesExecutor _getServiceCategoriesExecutor;
    private readonly IGetServiceCategoryByIdExecutor _getServiceCategoryByIdExecutor;
    private readonly ICreateServiceCategoryExecutor _createServiceCategoryExecutor;
    private readonly IUpdateServiceCategoryExecutor _updateServiceCategoryExecutor;
    private readonly IDeleteServiceCategoryExecutor _deleteServiceCategoryExecutor;

    public ServiceCategoriesController(
        IGetServiceCategoriesExecutor getServiceCategoriesExecutor,
        IGetServiceCategoryByIdExecutor getServiceCategoryByIdExecutor,
        ICreateServiceCategoryExecutor createServiceCategoryExecutor,
        IUpdateServiceCategoryExecutor updateServiceCategoryExecutor,
        IDeleteServiceCategoryExecutor deleteServiceCategoryExecutor)
    {
        _getServiceCategoriesExecutor = getServiceCategoriesExecutor;
        _getServiceCategoryByIdExecutor = getServiceCategoryByIdExecutor;
        _createServiceCategoryExecutor = createServiceCategoryExecutor;
        _updateServiceCategoryExecutor = updateServiceCategoryExecutor;
        _deleteServiceCategoryExecutor = deleteServiceCategoryExecutor;
    }

    /// <summary>
    /// Lists service categories. CentralOffice sees all; BranchAdmin/Staff/Customer see only their branch.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServiceCategories(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getServiceCategoriesExecutor.ExecuteAsync(
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

    /// <summary>Returns a service category by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceCategoryById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getServiceCategoryByIdExecutor.ExecuteAsync(
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

    /// <summary>Creates a service category for a branch. CentralOffice or BranchAdmin of that branch.</summary>
    [HttpPost]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(ServiceCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateServiceCategory(
        [FromBody] CreateServiceCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createServiceCategoryExecutor.ExecuteAsync(
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

    /// <summary>Updates a service category. CentralOffice or BranchAdmin of that category's branch.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(ServiceCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateServiceCategory(
        Guid id,
        [FromBody] UpdateServiceCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateServiceCategoryExecutor.ExecuteAsync(
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

    /// <summary>Deletes a service category that has no services. CentralOffice or BranchAdmin of that branch.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteServiceCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _deleteServiceCategoryExecutor.ExecuteAsync(
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
