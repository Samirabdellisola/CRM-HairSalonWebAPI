using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
}
