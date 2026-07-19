using System.ComponentModel.DataAnnotations;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Payments.DTOs;

public class CreatePaymentRequest
{
    [Required]
    public Guid OrderId { get; set; }

    /// <summary>
    /// Payment method id: CreditCard=0, Cash=1, BankTransfer=2, DebitCard=3, PosTerminal=4.
    /// </summary>
    [Required, EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; set; }

    public DateTime? Date { get; set; }
}
