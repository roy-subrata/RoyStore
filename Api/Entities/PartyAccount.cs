namespace Api.Entities;

public class PartyAccount : BaseEntity
{
    public string PartyType { get; set; } = null!; // "Customer" or "Supplier"
    public required string PartyId { get; set; } // FK to Customer.Id or Supplier.Id

    public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
}