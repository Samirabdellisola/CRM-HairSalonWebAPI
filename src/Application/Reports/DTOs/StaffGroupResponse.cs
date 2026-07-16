namespace SalonCRM.Application.Reports.DTOs;

public class StaffGroupResponse<TItem>
{
    public Guid StaffId { get; set; }

    public string StaffName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public int Count { get; set; }

    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
}
