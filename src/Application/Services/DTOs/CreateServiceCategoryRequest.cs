using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Services.DTOs;

public class CreateServiceCategoryRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid BranchId { get; set; }
}
