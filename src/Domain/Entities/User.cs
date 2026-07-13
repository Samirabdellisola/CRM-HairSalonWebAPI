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

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional contact phone number. Not required.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Optional contact address. Not required.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Associates Staff/BranchAdmin accounts to a branch. Not used for Customer/CentralOffice.
    /// </summary>
    public Guid? BranchId { get; set; }

    public Branch? Branch { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}
