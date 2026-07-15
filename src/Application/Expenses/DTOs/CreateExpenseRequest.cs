using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Expenses.DTOs;

public class CreateExpenseRequest
{
    [Required]
    public Guid ExpenseCategoryId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public DateTime? Date { get; set; }
}
