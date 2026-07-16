namespace SalonCRM.Application.Reports.DTOs;

public class TopDayResponse
{
    public DateOnly Date { get; set; }

    public decimal Revenue { get; set; }

    public int PaymentCount { get; set; }
}
