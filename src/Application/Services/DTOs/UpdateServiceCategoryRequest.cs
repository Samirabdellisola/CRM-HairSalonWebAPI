using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Services.DTOs;

public class UpdateServiceCategoryRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
