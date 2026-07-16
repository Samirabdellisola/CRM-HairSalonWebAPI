using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Branches.DTOs;

public class CreateBranchRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string AdminName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string AdminPassword { get; set; } = string.Empty;
}
