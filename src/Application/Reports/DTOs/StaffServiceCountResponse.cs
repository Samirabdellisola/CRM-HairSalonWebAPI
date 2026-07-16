namespace SalonCRM.Application.Reports.DTOs;

public class StaffServiceCountResponse
{
    public Guid StaffId { get; set; }

    public string StaffName { get; set; } = string.Empty;

    public Guid ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
