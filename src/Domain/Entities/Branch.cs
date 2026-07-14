using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// A salon branch/location. Each branch may have one BranchAdmin user assigned to it.
/// </summary>
public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates the branch is frozen (e.g. contract ended or temporarily suspended).
    /// </summary>
    public bool IsFrozen { get; set; }

    /// <summary>
    /// The BranchAdmin user assigned to manage this branch.
    /// </summary>
    public Guid? AdminId { get; set; }

    public User? AdminUser { get; set; }
}
