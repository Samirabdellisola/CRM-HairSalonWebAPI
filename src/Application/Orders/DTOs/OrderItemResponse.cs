namespace SalonCRM.Application.Orders.DTOs;

public class OrderItemResponse
{
    public Guid Id { get; set; }

    public Guid ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public decimal ServicePrice { get; set; }
}
