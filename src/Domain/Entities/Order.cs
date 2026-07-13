using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Guid? StaffId { get; set; }

    public Staff? Staff { get; set; }

    public Guid? BranchId { get; set; }

    public Branch? Branch { get; set; }

    public Guid? ServiceId { get; set; }

    public Service? Service { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string Status { get; set; } = "Pending";

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
