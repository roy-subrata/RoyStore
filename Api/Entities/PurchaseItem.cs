namespace Api.Entities;

public class PurchaseItem : BaseEntity
{
    // References
    public string? ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string? PurchaseId { get; set; } = null!;
    public Purchase Purchase { get; set; } = null!;

    // Quantities
    public double OrderedQuantity { get; set; }        // Total quantity ordered in purchase unit
    public double ReceivedQuantity { get; set; }       // Quantity received so far

    // Unit info
    public string UnitId { get; set; }                     // Purchase unit
    public Unit Unit { get; set; } = null!;

    // Conversion factor to product's base unit (stored at purchase time)
    public double UnitConversion { get; set; } = 1;

    // Price
    public double UnitPrice { get; set; }             // Price per purchase unit
    public double TotalPrice => OrderedQuantity * UnitPrice;

    // Add received quantity to stock
    public void AddToStock(double quantityToAdd)
    {
        if (quantityToAdd <= 0)
            throw new ArgumentException("Received quantity must be positive.");

        // Convert to base unit
        var baseQty = quantityToAdd * UnitConversion;

        // Update product stock
        Product.StockQuantity += baseQty;

        // Update received quantity
        ReceivedQuantity += quantityToAdd;

        // Prevent exceeding ordered quantity
        if (ReceivedQuantity > OrderedQuantity)
            ReceivedQuantity = OrderedQuantity;
    }

    // Remaining quantity to receive
    public double RemainingQuantity => OrderedQuantity - ReceivedQuantity;

    // Check if fully received
    public bool IsFullyReceived => ReceivedQuantity >= OrderedQuantity;
}
