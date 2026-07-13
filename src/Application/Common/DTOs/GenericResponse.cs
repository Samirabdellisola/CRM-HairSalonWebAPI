namespace SalonCRM.Application.Common.DTOs;

public class GenericResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public static GenericResponse Ok(string message) => new() { Success = true, Message = message };
}
