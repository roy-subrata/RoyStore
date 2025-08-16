namespace Api.Entities;

public class SaleReturnItem:BaseEntity
{
    public int SaleReturnId { get; set; }
    public SaleReturn SaleReturn { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Item { get; set; } = null!;

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice => Quantity * UnitPrice;
}

public class PurchaseReturn:BaseEntity
{
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;

    public DateTime ReturnDate { get; set; }
    public string? Reason { get; set; }

    public ICollection<PaymentTransaction> Payments { get; set; } = [];
}