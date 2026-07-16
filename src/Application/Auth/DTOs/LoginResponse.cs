namespace SalonCRM.Application.Auth.DTOs;

public class LoginResponse
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Branch of the user. Null only for CentralOffice.
    /// </summary>
    public Guid? BranchId { get; set; }

    public AuthTokensResponse Tokens { get; set; } = new();
}
