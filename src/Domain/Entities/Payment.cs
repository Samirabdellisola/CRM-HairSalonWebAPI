using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// A payment recorded for a salon order.
/// </summary>
public class Payment : BaseEntity
{
    public string PaymentMethod { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public User Customer { get; set; } = null!;

    public Guid StaffId { get; set; }

    public User Staff { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public DateTime Date { get; set; }
}
