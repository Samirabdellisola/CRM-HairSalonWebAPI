namespace SalonCRM.Application.Customers.DTOs;

/// <summary>
/// File payload for customer photo upload. Controllers map IFormFile into this DTO.
/// </summary>
public class UploadCustomerPhotoRequest
{
    public required Stream Content { get; set; }

    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public long Length { get; set; }
}
