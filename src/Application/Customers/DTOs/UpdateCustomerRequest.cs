using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Customers.DTOs;

/// <summary>
/// Fields a caller can update on a Customer user. Only non-null fields are applied.
/// Password and role cannot be changed via this request.
/// </summary>
public class UpdateCustomerRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateOnly? Birthday { get; set; }
}
