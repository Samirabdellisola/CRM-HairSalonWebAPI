using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Customers.DTOs;

/// <summary>
/// Updates HairColor and Preferences. Photo is updated only via the photo upload endpoint.
/// </summary>
public class UpdateProfileRequest
{
    [MaxLength(100)]
    public string? HairColor { get; set; }

    [MaxLength(2000)]
    public string? Preferences { get; set; }
}
