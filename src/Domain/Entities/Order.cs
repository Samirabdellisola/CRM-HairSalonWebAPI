using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// A salon order linking a customer, a staff member, and one or more services.
/// </summary>
public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }

    public User Customer { get; set; } = null!;

    public Guid StaffId { get; set; }

    public User Staff { get; set; } = null!;

    /// <summary>
    /// Denormalized from the staff member's branch at creation time.
    /// </summary>
    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public bool Completed { get; set; }

    public bool Cancelled { get; set; }

    /// <summary>
    /// Optional payment reference. Payment entity will be added later; no FK yet.
    /// </summary>
    public Guid? PaymentId { get; set; }

    public string? Comment { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
