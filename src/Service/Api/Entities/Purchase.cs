namespace Api.Entities;

public class Purchase:BaseEntity
{
    public string PurchaseNumber { get; set; } = null!;
    public string SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount => TotalAmount - PaidAmount;
    
    public Status Status { get; set; }

    public ICollection<PaymentTransaction> Payments { get; set; } = [];
}

public enum Status
{
    Draft,
    Pending,
    Done
}