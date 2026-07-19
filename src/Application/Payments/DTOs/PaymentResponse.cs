using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Payments.DTOs;

public class PaymentResponse
{
    public Guid Id { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public Guid CustomerId { get; set; }

    public Guid StaffId { get; set; }

    public Guid BranchId { get; set; }

    public Guid OrderId { get; set; }

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
