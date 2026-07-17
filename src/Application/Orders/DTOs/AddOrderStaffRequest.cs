using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Orders.DTOs;

public class AddOrderStaffRequest
{
    [Required]
    public Guid StaffId { get; set; }
}
