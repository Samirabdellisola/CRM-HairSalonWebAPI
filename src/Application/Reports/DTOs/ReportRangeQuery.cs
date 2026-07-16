using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Reports.DTOs;

public class ReportRangeQuery
{
    public ReportRangeType RangeType { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public Guid? BranchId { get; set; }
}
