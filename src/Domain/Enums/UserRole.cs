namespace SalonCRM.Domain.Enums;

/// <summary>
/// Roles supported by the Salon CRM auth system. Stored as string in the database.
/// </summary>
public enum UserRole
{
    Customer,
    Staff,
    BranchAdmin,
    CentralOffice
}
