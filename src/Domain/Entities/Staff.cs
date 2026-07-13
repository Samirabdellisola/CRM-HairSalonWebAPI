using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

public class Staff : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public Guid? BranchId { get; set; }

    public Branch? Branch { get; set; }

    public bool IsActive { get; set; } = true;
}
