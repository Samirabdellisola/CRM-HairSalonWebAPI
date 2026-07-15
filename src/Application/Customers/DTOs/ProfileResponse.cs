namespace SalonCRM.Application.Customers.DTOs;

public class ProfileResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? HairColor { get; set; }

    public string? PhotoPath { get; set; }

    public string? Preferences { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
