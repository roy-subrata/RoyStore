namespace Api.Entities;

public class PurchaseItem : BaseEntity
{
    public string? ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public Purchase Purchase { get; set; }
    public string? PurchaseId { get; set; }= null!;
    public decimal Quantity { get; set; }            // Purchased quantity in selected unit
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;          // Unit used in purchase
    public decimal UnitConversion { get; set; } = 1; // To convert to base unit
    public decimal UnitPrice { get; set; }           // Price per unit (purchase unit)
    public decimal TotalPrice => Quantity * UnitPrice;

    // Update stock in base unit
    public void AddToStock() => Product.StockQuantity += Quantity * UnitConversion;
}