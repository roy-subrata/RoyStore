namespace Api.Entities;

public class PurchaseReturnItem:BaseEntity  
{
    public int PurchaseReturnId { get; set; }
    public PurchaseReturn PurchaseReturn { get; set; } = null!;

    public int ProductId  { get; set; }
    public Product Product { get; set; } = null!;

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice => Quantity * UnitPrice;
}