using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Services.DTOs;

public class UpdateServiceRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? ImagePath { get; set; }

    public Guid? ServiceCategoryId { get; set; }
}
