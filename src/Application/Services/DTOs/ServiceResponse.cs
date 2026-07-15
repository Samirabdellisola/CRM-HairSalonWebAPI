namespace SalonCRM.Application.Services.DTOs;

public class ServiceResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? ImagePath { get; set; }

    public Guid BranchId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
