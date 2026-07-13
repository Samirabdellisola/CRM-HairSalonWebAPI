using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// Single-use token issued for the forgot-password / reset-password flow.
/// </summary>
public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public bool IsValid => UsedAt is null && ExpiresAt > DateTime.UtcNow;
}
