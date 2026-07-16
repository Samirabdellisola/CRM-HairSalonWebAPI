namespace SalonCRM.Application.Reports.DTOs;

public class TopServiceResponse
{
    public Guid ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Revenue { get; set; }
}
