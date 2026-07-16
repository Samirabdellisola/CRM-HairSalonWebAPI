using SalonCRM.Domain.Common;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// Authentication identity for the Salon CRM. Represents a login account with a role.
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates the staff account is frozen (e.g. contract ended).
    /// </summary>
    public bool IsFrozen { get; set; }

    /// <summary>
    /// Optional contact phone number. Not required.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Optional contact address. Not required.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Optional birthday. Used for special-dates listings.
    /// </summary>
    public DateOnly? Birthday { get; set; }

    /// <summary>
    /// Required for Staff, BranchAdmin, and Customer. Null only for CentralOffice.
    /// </summary>
    public Guid? BranchId { get; set; }

    public Branch? Branch { get; set; }

    public Profile? Profile { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}
