using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Services.DTOs;

public class CreateServiceRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public Guid BranchId { get; set; }

    [MaxLength(500)]
    public string? ImagePath { get; set; }
}
