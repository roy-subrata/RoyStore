namespace Api.Entities;

public class PaymentMethod : BaseEntity
{
    public string ProviderName { get; set; } = null!;
    public string AccountNo { get; set; } = null!;
    public string AccountOwner { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}