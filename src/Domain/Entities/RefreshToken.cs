using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// Opaque, long-lived refresh token used to rotate short-lived JWT access tokens.
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
