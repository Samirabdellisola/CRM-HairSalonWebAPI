using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// Service category scoped to a branch.
/// </summary>
public class ServiceCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public ICollection<Service> Services { get; set; } = new List<Service>();
}
