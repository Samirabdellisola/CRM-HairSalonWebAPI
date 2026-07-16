namespace SalonCRM.Application.Reports.DTOs;

public class TopCustomerResponse
{
    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal Revenue { get; set; }

    public int PaymentCount { get; set; }
}
