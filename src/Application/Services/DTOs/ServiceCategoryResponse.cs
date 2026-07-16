namespace SalonCRM.Application.Services.DTOs;

public class ServiceCategoryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid BranchId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
