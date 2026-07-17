using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Orders.DTOs;

public class UpdateOrderRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Optional staff. Null clears the staff assignment on the order.
    /// </summary>
    public Guid? StaffId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }
}
