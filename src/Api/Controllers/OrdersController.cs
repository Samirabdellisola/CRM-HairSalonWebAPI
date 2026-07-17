using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Order management: list/get/create/update, add staff, complete/cancel,
/// and pending-payment listings. Each order has a single service.
/// </summary>
[Route("orders")]
[Produces("application/json")]
[Authorize(Roles = "CentralOffice,BranchAdmin,Staff,Customer")]
public class OrdersController : ApiControllerBase
{
    private readonly IGetOrdersExecutor _getOrdersExecutor;
    private readonly IGetOrderByIdExecutor _getOrderByIdExecutor;
    private readonly ICreateOrderExecutor _createOrderExecutor;
    private readonly IUpdateOrderExecutor _updateOrderExecutor;
    private readonly IAddOrderStaffExecutor _addOrderStaffExecutor;
    private readonly ICompleteOrderExecutor _completeOrderExecutor;
    private readonly ICancelOrderExecutor _cancelOrderExecutor;
    private readonly IGetPendingPaymentOrdersExecutor _getPendingPaymentOrdersExecutor;
    private readonly IGetOrderPaymentsExecutor _getOrderPaymentsExecutor;

    public OrdersController(
        IGetOrdersExecutor getOrdersExecutor,
        IGetOrderByIdExecutor getOrderByIdExecutor,
        ICreateOrderExecutor createOrderExecutor,
        IUpdateOrderExecutor updateOrderExecutor,
        IAddOrderStaffExecutor addOrderStaffExecutor,
        ICompleteOrderExecutor completeOrderExecutor,
        ICancelOrderExecutor cancelOrderExecutor,
        IGetPendingPaymentOrdersExecutor getPendingPaymentOrdersExecutor,
        IGetOrderPaymentsExecutor getOrderPaymentsExecutor)
    {
        _getOrdersExecutor = getOrdersExecutor;
        _getOrderByIdExecutor = getOrderByIdExecutor;
        _createOrderExecutor = createOrderExecutor;
        _updateOrderExecutor = updateOrderExecutor;
        _addOrderStaffExecutor = addOrderStaffExecutor;
        _completeOrderExecutor = completeOrderExecutor;
        _cancelOrderExecutor = cancelOrderExecutor;
        _getPendingPaymentOrdersExecutor = getPendingPaymentOrdersExecutor;
        _getOrderPaymentsExecutor = getOrderPaymentsExecutor;
    }

    /// <summary>
    /// Lists orders scoped by role: CentralOffice all, BranchAdmin by branch,
    /// Staff own orders, Customer own orders.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getOrdersExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Lists orders without a PaymentId (pending payment).</summary>
    [HttpGet("pending-payment")]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingPaymentOrders(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getPendingPaymentOrdersExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Returns an order by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getOrderByIdExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Lists payments for an order. Same visibility as viewing the order.</summary>
    [HttpGet("{orderId:guid}/payments")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderPayments(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _getOrderPaymentsExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), orderId, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Creates an order with a single service. Staff, BranchAdmin, or CentralOffice.</summary>
    [HttpPost]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createOrderExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Updates CustomerId, ServiceId, optional StaffId, and Comment on an open order.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _updateOrderExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Assigns a staff member to an open order.</summary>
    [HttpPatch("{id:guid}/add-staff")]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddOrderStaff(Guid id, [FromBody] AddOrderStaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _addOrderStaffExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Marks an order as completed.</summary>
    [HttpPatch("{id:guid}/complete")]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteOrder(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _completeOrderExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Marks an order as cancelled.</summary>
    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _cancelOrderExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }
}
