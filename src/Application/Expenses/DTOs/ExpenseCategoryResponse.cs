namespace SalonCRM.Application.Expenses.DTOs;

public class ExpenseCategoryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid BranchId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
