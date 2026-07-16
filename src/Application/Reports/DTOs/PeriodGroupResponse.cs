namespace SalonCRM.Application.Reports.DTOs;

public class PeriodGroupResponse<TItem>
{
    public string PeriodKey { get; set; } = string.Empty;

    public string PeriodLabel { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public int Count { get; set; }

    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
}
