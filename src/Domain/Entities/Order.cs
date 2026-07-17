using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// A salon order linking a customer, optional staff member, and a single service.
/// </summary>
public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }

    public User Customer { get; set; } = null!;

    /// <summary>
    /// Optional staff assigned to the order. May be set at creation or via add-staff.
    /// </summary>
    public Guid? StaffId { get; set; }

    public User? Staff { get; set; }

    /// <summary>
    /// Denormalized from the customer's branch at creation time.
    /// </summary>
    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public Guid ServiceId { get; set; }

    public Service Service { get; set; } = null!;

    /// <summary>
    /// Service name snapshotted at order creation/update time.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Service price snapshotted at order creation/update time.
    /// </summary>
    public decimal ServicePrice { get; set; }

    /// <summary>
    /// Kept in sync with ServicePrice for reporting/payment compatibility.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Business date of the order (UTC).
    /// </summary>
    public DateTime Date { get; set; }

    public bool Completed { get; set; }

    public bool Cancelled { get; set; }

    /// <summary>
    /// Optional payment reference once a payment has been created for this order.
    /// </summary>
    public Guid? PaymentId { get; set; }

    public Payment? Payment { get; set; }

    public string? Comment { get; set; }
}
