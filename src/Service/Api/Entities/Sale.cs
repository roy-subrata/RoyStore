namespace Api.Entities;

public class Sale:BaseEntity
{
    public string SaleNumber { get; set; } = null!;
    public string CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount => TotalAmount - PaidAmount;
    public Status Status { get; set; }
    public ICollection<PaymentTransaction> Payments { get; set; } = [];
}