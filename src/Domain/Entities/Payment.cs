using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order? Order { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = "Cash";

    public string Status { get; set; } = "Pending";

    public DateTime? PaidAt { get; set; }
}
