using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Payment management: list/get payments and create a payment for an order.
/// </summary>
[Route("payments")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin,Staff,Customer")]
public class PaymentsController : ApiControllerBase
{
    private readonly IGetPaymentsExecutor _getPaymentsExecutor;
    private readonly IGetPaymentByIdExecutor _getPaymentByIdExecutor;
    private readonly ICreatePaymentExecutor _createPaymentExecutor;

    public PaymentsController(
        IGetPaymentsExecutor getPaymentsExecutor,
        IGetPaymentByIdExecutor getPaymentByIdExecutor,
        ICreatePaymentExecutor createPaymentExecutor)
    {
        _getPaymentsExecutor = getPaymentsExecutor;
        _getPaymentByIdExecutor = getPaymentByIdExecutor;
        _createPaymentExecutor = createPaymentExecutor;
    }

    /// <summary>
    /// Lists payments scoped by role: CentralOffice all, BranchAdmin by branch,
    /// Staff own staff payments, Customer own customer payments.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayments(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getPaymentsExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Returns a payment by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getPaymentByIdExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Creates a payment for an order and links it via Order.PaymentId.</summary>
    [HttpPost]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createPaymentExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
