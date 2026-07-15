namespace SalonCRM.Application.Orders.DTOs;

public class OrderResponse
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid StaffId { get; set; }

    public Guid BranchId { get; set; }

    public decimal TotalPrice { get; set; }

    public bool Completed { get; set; }

    public bool Cancelled { get; set; }

    public Guid? PaymentId { get; set; }

    public string? Comment { get; set; }

    public IReadOnlyList<OrderItemResponse> Items { get; set; } = Array.Empty<OrderItemResponse>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
