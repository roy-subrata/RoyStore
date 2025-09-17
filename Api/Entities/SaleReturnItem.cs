// using Api.Entities.Experiment;

// namespace Api.Entities;

// public class SaleReturnItem:BaseEntity
// {
//     public int SaleReturnId { get; set; }
//     public SaleReturn SaleReturn { get; set; } = null!;

//     public int ProductId { get; set; }
//     public Product Item { get; set; } = null!;

//     public int UnitId { get; set; }
//     public Unit Unit { get; set; } = null!;

//     public decimal Quantity { get; set; }

//     public decimal UnitPrice { get; set; }

//     public decimal TotalPrice => Quantity * UnitPrice;
// }

// public class PurchaseReturn:BaseEntity
// {
//     public int PurchaseId { get; set; }
//     public Purchase Purchase { get; set; } = null!;

//     public DateTime ReturnDate { get; set; }
//     public string? Reason { get; set; }

//     public ICollection<PaymentTransaction> Payments { get; set; } = [];
// }

namespace Api.Entities;

public class SaleReturnItem : BaseEntity
{
    public string? ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;          // Unit used in purchase
    public double UnitConversion { get; set; } = 1; // To convert to base unit
    public double RefundPrice { get; set; }          // Actual refund price per unit
    public double RefundAmount => Quantity * RefundPrice;
    // Add stock back on return

    public void AddToStock() => Product.StockQuantity += Quantity * UnitConversion;
}