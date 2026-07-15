using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Application.Customers.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Customer management: list/get/update customers, profiles, photo upload, and special dates.
/// </summary>
[Route("customers")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin,Customer")]
public class CustomersController : ApiControllerBase
{
    private readonly IGetCustomersExecutor _getCustomersExecutor;
    private readonly IGetCustomerByIdExecutor _getCustomerByIdExecutor;
    private readonly IUpdateCustomerExecutor _updateCustomerExecutor;
    private readonly IGetCustomerProfileExecutor _getCustomerProfileExecutor;
    private readonly IUpdateCustomerProfileExecutor _updateCustomerProfileExecutor;
    private readonly IUploadCustomerPhotoExecutor _uploadCustomerPhotoExecutor;
    private readonly IGetSpecialDatesExecutor _getSpecialDatesExecutor;

    public CustomersController(
        IGetCustomersExecutor getCustomersExecutor,
        IGetCustomerByIdExecutor getCustomerByIdExecutor,
        IUpdateCustomerExecutor updateCustomerExecutor,
        IGetCustomerProfileExecutor getCustomerProfileExecutor,
        IUpdateCustomerProfileExecutor updateCustomerProfileExecutor,
        IUploadCustomerPhotoExecutor uploadCustomerPhotoExecutor,
        IGetSpecialDatesExecutor getSpecialDatesExecutor)
    {
        _getCustomersExecutor = getCustomersExecutor;
        _getCustomerByIdExecutor = getCustomerByIdExecutor;
        _updateCustomerExecutor = updateCustomerExecutor;
        _getCustomerProfileExecutor = getCustomerProfileExecutor;
        _updateCustomerProfileExecutor = updateCustomerProfileExecutor;
        _uploadCustomerPhotoExecutor = uploadCustomerPhotoExecutor;
        _getSpecialDatesExecutor = getSpecialDatesExecutor;
    }

    /// <summary>
    /// Lists Customer users. CentralOffice sees all; BranchAdmin sees only customers of their branch.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getCustomersExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Lists Customer users with a birthday in the current month.
    /// </summary>
    [HttpGet("special-dates")]
    [Authorize(Roles = "CentralOffice,BranchAdmin")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpecialDates(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getSpecialDatesExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Returns a Customer user by id. CentralOffice, BranchAdmin of that customer's branch, or the customer themselves.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getCustomerByIdExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>
    /// Updates Email/Phone/Address/Birthday on a Customer. Does not change password or role.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateCustomerExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Returns the profile for a Customer user.</summary>
    [HttpGet("{id:guid}/profile")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerProfile(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getCustomerProfileExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Updates HairColor and Preferences on a Customer profile. Photo is updated via POST photo.</summary>
    [HttpPut("{id:guid}/profile")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomerProfile(Guid id, [FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateCustomerProfileExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Uploads a customer profile photo (JPEG, PNG, or WebP, max 5 MB).</summary>
    [HttpPost("{id:guid}/photo")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadCustomerPhoto(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new GenericResponse { Success = false, Message = "Photo file is required." });
            }

            await using var stream = file.OpenReadStream();
            var request = new UploadCustomerPhotoRequest
            {
                Content = stream,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length
            };

            var response = await _uploadCustomerPhotoExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
