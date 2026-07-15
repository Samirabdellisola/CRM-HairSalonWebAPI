using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// A service line on an order, with name/price snapshotted at add time.
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public Guid ServiceId { get; set; }

    public Service Service { get; set; } = null!;

    public string ServiceName { get; set; } = string.Empty;

    public decimal ServicePrice { get; set; }
}
