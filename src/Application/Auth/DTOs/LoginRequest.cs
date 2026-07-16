using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Auth.DTOs;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Optional branch scope. When set, authenticates a user of that branch.
    /// When omitted, authenticates the Central Office account only.
    /// </summary>
    public Guid? BranchId { get; set; }
}
