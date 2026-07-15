using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;

namespace SalonCRM.Api.Controllers;

/// <summary>
/// Order management: list/get/create/update, add/remove services, complete/cancel,
/// and pending-payment listings.
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
    private readonly IAddOrderServiceExecutor _addOrderServiceExecutor;
    private readonly IRemoveOrderServiceExecutor _removeOrderServiceExecutor;
    private readonly ICompleteOrderExecutor _completeOrderExecutor;
    private readonly ICancelOrderExecutor _cancelOrderExecutor;
    private readonly IGetPendingPaymentOrdersExecutor _getPendingPaymentOrdersExecutor;

    public OrdersController(
        IGetOrdersExecutor getOrdersExecutor,
        IGetOrderByIdExecutor getOrderByIdExecutor,
        ICreateOrderExecutor createOrderExecutor,
        IUpdateOrderExecutor updateOrderExecutor,
        IAddOrderServiceExecutor addOrderServiceExecutor,
        IRemoveOrderServiceExecutor removeOrderServiceExecutor,
        ICompleteOrderExecutor completeOrderExecutor,
        ICancelOrderExecutor cancelOrderExecutor,
        IGetPendingPaymentOrdersExecutor getPendingPaymentOrdersExecutor)
    {
        _getOrdersExecutor = getOrdersExecutor;
        _getOrderByIdExecutor = getOrderByIdExecutor;
        _createOrderExecutor = createOrderExecutor;
        _updateOrderExecutor = updateOrderExecutor;
        _addOrderServiceExecutor = addOrderServiceExecutor;
        _removeOrderServiceExecutor = removeOrderServiceExecutor;
        _completeOrderExecutor = completeOrderExecutor;
        _cancelOrderExecutor = cancelOrderExecutor;
        _getPendingPaymentOrdersExecutor = getPendingPaymentOrdersExecutor;
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

    /// <summary>Creates an order. Staff, BranchAdmin, or CentralOffice.</summary>
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

    /// <summary>Updates CustomerId, StaffId, and Comment on an open order.</summary>
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

    /// <summary>Adds a service line to an order.</summary>
    [HttpPatch("{id:guid}/add-service")]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddOrderService(Guid id, [FromBody] AddOrderServiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _addOrderServiceExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
            return Ok(response);
        }
        catch (AppException ex)
        {
            return HandleAppException(ex);
        }
    }

    /// <summary>Removes a service line from an order.</summary>
    [HttpPatch("{id:guid}/remove-service")]
    [Authorize(Roles = "CentralOffice,BranchAdmin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveOrderService(Guid id, [FromBody] RemoveOrderServiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _removeOrderServiceExecutor.ExecuteAsync(GetCurrentUserId(), GetCurrentUserRole(), id, request, cancellationToken);
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
