using SalonCRM.Domain.Common;

namespace SalonCRM.Domain.Entities;

/// <summary>
/// Customer profile data linked 1:1 to a User with Role = Customer.
/// </summary>
public class Profile : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string? HairColor { get; set; }

    /// <summary>
    /// Relative URL path to the uploaded photo (e.g. /uploads/profiles/{userId}/file.jpg).
    /// </summary>
    public string? PhotoPath { get; set; }

    public string? Preferences { get; set; }
}
