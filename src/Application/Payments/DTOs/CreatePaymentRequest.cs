using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Payments.DTOs;

public class CreatePaymentRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required, MaxLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    public DateTime? Date { get; set; }
}
