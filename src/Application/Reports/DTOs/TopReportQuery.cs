namespace SalonCRM.Application.Reports.DTOs;

public class TopReportQuery
{
    public int Limit { get; set; } = 10;

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public Guid? BranchId { get; set; }
}
