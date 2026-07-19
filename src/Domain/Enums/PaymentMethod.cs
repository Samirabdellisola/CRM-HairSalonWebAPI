namespace SalonCRM.Domain.Enums;

/// <summary>
/// Payment method identifiers accepted by the API and stored as integers.
/// </summary>
public enum PaymentMethod
{
    CreditCard = 0,
    Cash = 1,
    BankTransfer = 2,
    DebitCard = 3,
    PosTerminal = 4
}
