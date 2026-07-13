using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Auth.DTOs;

public class RegisterCustomerRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }
}
