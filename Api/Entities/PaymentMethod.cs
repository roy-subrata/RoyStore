namespace Api.Entities;

public class PaymentMethod : BaseEntity
{
    public int PaymentTypeId { get; set; }
    public PaymentType Type { get; set; } = null!;

    public string Provider { get; set; } = null!; // "bKash", "Nagad", "Visa"
    public string? ReferenceCode { get; set; }    // Merchant ID, account no, etc.
    public bool IsActive { get; set; } = true;
}