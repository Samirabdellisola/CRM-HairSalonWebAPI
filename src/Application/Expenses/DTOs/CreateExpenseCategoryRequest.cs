using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Expenses.DTOs;

public class CreateExpenseCategoryRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid BranchId { get; set; }
}
