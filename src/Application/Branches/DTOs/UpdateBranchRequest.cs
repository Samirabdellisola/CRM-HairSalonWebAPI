using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Branches.DTOs;

public class UpdateBranchRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Phone { get; set; } = string.Empty;
}
