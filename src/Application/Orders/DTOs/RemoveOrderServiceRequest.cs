using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Orders.DTOs;

public class RemoveOrderServiceRequest
{
    [Required]
    public Guid ServiceId { get; set; }
}
