using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Users.DTOs;

/// <summary>
/// Fields a caller can update on a user. Only non-null fields are applied.
/// </summary>
public class UpdateUserRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
}
