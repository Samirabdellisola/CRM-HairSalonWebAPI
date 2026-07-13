using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
}
