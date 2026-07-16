using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Customers.DTOs;

public class CustomerResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Guid? BranchId { get; set; }

    public DateOnly? Birthday { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
