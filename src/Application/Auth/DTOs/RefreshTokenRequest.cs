using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Auth.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
