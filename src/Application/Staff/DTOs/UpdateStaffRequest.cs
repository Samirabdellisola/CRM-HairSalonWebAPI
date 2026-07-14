using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Staff.DTOs;

/// <summary>
/// Fields a caller can update on a Staff user. Only non-null fields are applied.
/// Password and role cannot be changed via this request.
/// </summary>
public class UpdateStaffRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
}
