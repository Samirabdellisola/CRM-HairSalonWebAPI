using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Orders.DTOs;

public class CreateOrderRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Optional staff for the order. May also be assigned later via add-staff.
    /// </summary>
    public Guid? StaffId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Business date of the order. Defaults to UTC now when omitted.
    /// </summary>
    public DateTime? Date { get; set; }
}
