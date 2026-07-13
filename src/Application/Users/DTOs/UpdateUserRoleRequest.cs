using System.ComponentModel.DataAnnotations;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Users.DTOs;

public class UpdateUserRoleRequest
{
    [Required]
    public UserRole NewRole { get; set; }
}
