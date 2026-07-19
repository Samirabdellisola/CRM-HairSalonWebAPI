using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Orders.DTOs;

public class RemoveOrderStaffRequest
{
    [Required]
    public Guid StaffId { get; set; }
}
