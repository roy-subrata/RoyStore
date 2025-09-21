namespace Api.Entities;

public class Purchase : BaseEntity
{
    public string PurchaseNumber { get; set; } = null!;
    public string SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
    public PurchaseStatus Status { get; set; }
    public string ShipTo { get; set; }
    public double DiscountAmount { get; set; }
    public double Vat { get; set; }
    public double Tax{ get; set; }
    public double DeliveryCharge { get; set; }
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}