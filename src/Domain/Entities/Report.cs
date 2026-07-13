using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// Placeholder for persisted aggregate metrics / report snapshots.
/// </summary>
public class Report : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string ReportType { get; set; } = string.Empty;

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public string? PayloadJson { get; set; }

    public Guid? BranchId { get; set; }

    public Branch? Branch { get; set; }
}
