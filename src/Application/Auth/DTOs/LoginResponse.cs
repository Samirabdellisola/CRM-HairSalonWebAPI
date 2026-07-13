namespace SalonCRM.Application.Auth.DTOs;

public class LoginResponse
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public AuthTokensResponse Tokens { get; set; } = new();
}
