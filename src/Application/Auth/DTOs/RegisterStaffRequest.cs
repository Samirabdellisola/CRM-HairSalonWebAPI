using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Auth.DTOs;

public class RegisterStaffRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Branch the new Staff user is assigned to. Required; when the caller is a
    /// BranchAdmin, it must match the branch they administer.
    /// </summary>
    [Required]
    public Guid BranchId { get; set; }
}
