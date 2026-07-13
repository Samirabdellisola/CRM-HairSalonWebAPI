using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

public class Expense : BaseEntity
{
    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? Category { get; set; }

    public Guid? BranchId { get; set; }

    public Branch? Branch { get; set; }

    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
}
