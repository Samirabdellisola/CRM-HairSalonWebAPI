using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// Expense category scoped to a branch. Managed by CentralOffice only.
/// </summary>
public class ExpenseCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
