using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Orders.DTOs;

public class CreateOrderRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid StaffId { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }

    public List<Guid>? ServiceIds { get; set; }
}
