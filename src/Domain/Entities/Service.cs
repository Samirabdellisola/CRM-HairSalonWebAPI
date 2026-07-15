using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// A billable salon service offered by a specific branch.
/// </summary>
public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    /// <summary>
    /// Relative or absolute URL to the service image.
    /// </summary>
    public string? ImagePath { get; set; }

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;
}
