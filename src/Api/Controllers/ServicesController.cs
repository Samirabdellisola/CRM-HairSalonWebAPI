using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Salon service catalog CRUD. Writes are restricted to CentralOffice and the
/// BranchAdmin of the service's branch.
/// </summary>
[Route("services")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin,Staff,Customer")]
public class ServicesController : ApiControllerBase
{
    private readonly IGetServicesExecutor _getServicesExecutor;
    private readonly IGetServiceByIdExecutor _getServiceByIdExecutor;
    private readonly ICreateServiceExecutor _createServiceExecutor;
    private readonly IUpdateServiceExecutor _updateServiceExecutor;
    private readonly IDeleteServiceExecutor _deleteServiceExecutor;

    public ServicesController(
        IGetServicesExecutor getServicesExecutor,
        IGetServiceByIdExecutor getServiceByIdExecutor,
        ICreateServiceExecutor createServiceExecutor,
        IUpdateServiceExecutor updateServiceExecutor,
        IDeleteServiceExecutor deleteServiceExecutor)
    {
        _getServicesExecutor = getServicesExecutor;
        _getServiceByIdExecutor = getServiceByIdExecutor;
        _createServiceExecutor = createServiceExecutor;
        _updateServiceExecutor = updateServiceExecutor;
        _deleteServiceExecutor = deleteServiceExecutor;
    }

    /// <summary>
    /// Lists services. CentralOffice sees all; BranchAdmin/Staff/Customer see only their branch.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getServicesExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Returns a service by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getServiceByIdExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Creates a service for a branch. CentralOffice or BranchAdmin of that branch.</summary>
    [HttpPost]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createServiceExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Updates a service. CentralOffice or BranchAdmin of that service's branch.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateServiceExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Deletes a service. CentralOffice or BranchAdmin of that service's branch.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteService(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _deleteServiceExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
