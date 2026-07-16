namespace SalonCRM.Domain.Enums;

/// <summary>
/// How report data is bucketed (turnover/expenses) or how the date window is resolved (staff reports).
/// </summary>
public enum ReportRangeType
{
    Day = 0,
    Month = 1,
    Year = 2,
    Custom = 3
}
