namespace Api.Entities;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}