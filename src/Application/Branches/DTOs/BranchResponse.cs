namespace SalonCRM.Application.Branches.DTOs;

public class BranchResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsFrozen { get; set; }

    /// <summary>
    /// Display name of the BranchAdmin. Null when no admin is assigned.
    /// </summary>
    public string? AdminName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
