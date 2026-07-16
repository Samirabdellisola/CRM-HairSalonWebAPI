namespace SalonCRM.Application.Reports.DTOs;

public class StaffClientCountResponse
{
    public Guid StaffId { get; set; }

    public string StaffName { get; set; } = string.Empty;

    public int ClientCount { get; set; }
}
