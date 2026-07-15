namespace SalonCRM.Application.Expenses.DTOs;

public class ExpenseResponse
{
    public Guid Id { get; set; }

    public Guid ExpenseCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public DateTime Date { get; set; }

    public Guid BranchId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
