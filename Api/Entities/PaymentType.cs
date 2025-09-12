namespace Api.Entities;

public class PaymentType : BaseEntity
{
    public string Code { get; set; } = null!;   // "CASH", "BANK", "MOBILE"
    public string Name { get; set; } = null!;   // "Cash", "Bank Transfer", etc.
    public bool IsActive { get; set; } = true;

    public ICollection<PaymentMethod> Methods { get; set; } = [];
}