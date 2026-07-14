using System.ComponentModel.DataAnnotations;

namespace SalonCRM.Application.Branches.DTOs;

public class AssignBranchAdminRequest
{
    [Required]
    public Guid UserId { get; set; }
}
