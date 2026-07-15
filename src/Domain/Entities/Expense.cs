using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// A branch expense linked to an expense category.
/// </summary>
public class Expense : BaseEntity
{
    public Guid ExpenseCategoryId { get; set; }

    public ExpenseCategory ExpenseCategory { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    /// <summary>
    /// Business date of the expense (UTC). Used by by-range queries.
    /// </summary>
    public DateTime Date { get; set; }
}
