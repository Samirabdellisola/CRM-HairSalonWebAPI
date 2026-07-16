namespace SalonCRM.Application.Reports.DTOs;

public class TopStaffResponse
{
    public Guid StaffId { get; set; }

    public string StaffName { get; set; } = string.Empty;

    public decimal Revenue { get; set; }

    public int PaymentCount { get; set; }
}
