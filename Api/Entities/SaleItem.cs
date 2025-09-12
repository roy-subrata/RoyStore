// namespace Api.Entities;

// public class SaleItem:BaseEntity
// {
//     public int SaleId { get; set; }
//     public Sale Sale { get; set; } = null!;

//     public string ProductId { get; set; }

//     public Product Product { get; set; } = null!;

//     public int Quantity { get; set; }

//     public decimal UnitPrice { get; set; }

//     public DateTime SaleDate => Sale.SaleDate;

// }

namespace Api.Entities;

public class SaleItem : BaseEntity
{
    public string? ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;          // Unit used in purchase
    public decimal UnitConversion { get; set; } = 1; // To convert to base unit

    // Reduce stock on sale
    public void ReduceStock() => Product.StockQuantity -= Quantity * UnitConversion;
}