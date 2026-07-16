using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.DTOs;

public class StaffResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Guid? BranchId { get; set; }

    public bool IsActive { get; set; }

    public bool IsFrozen { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
