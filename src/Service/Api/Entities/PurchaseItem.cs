namespace Api.Entities;

public class PurchaseItem:BaseEntity
{
    public string PurchaseId { get; set; } 
    public Purchase Purchase { get; set; } = null!;
    public string ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int RemainingQuantity { get; set; } // Actual stock at this price
    public DateTime PurchaseDate { get; set; }
}

public enum StockMovementType
{
    Purchase,
    Sale,
    ReturnIn,
    ReturnOut,
    Adjustment,
    TransferIn,
    TransferOut,
    WriteOff
}

public class StockMovement : BaseEntity
{
    public string ProductId { get; set; } = null!;
    public Product Product { get; set; } = null!;

    // Optional: link to specific purchase lot for lot-based tracking
    public string? PurchaseItemId { get; set; }
    public PurchaseItem? PurchaseItem { get; set; }

    public StockMovementType MovementType { get; set; }
    
    // Positive for in, negative for out
    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    // Links to source document (PO, SO, Adjustment, etc.)
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
}


