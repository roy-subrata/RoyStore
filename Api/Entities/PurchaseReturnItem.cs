namespace Api.Entities;

public class PurchaseReturnItem : BaseEntity
{
    public string? ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;          // Unit used in purchase
    public double UnitConversion { get; set; } = 1; // To convert to base unit
    // Reduce stock on return to supplier
    public void ReduceStock() => Product.StockQuantity -= Quantity * UnitConversion;

    public class ProductUnit : BaseEntity
    {
        public string? ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        public decimal ConversionToBase { get; set; } // e.g., 1 box = 10 pieces → 10
    }
    public class RewardRate : BaseEntity
    {
        public decimal Rate { get; set; } = 0.01m; // e.g., 1% of purchase
        public string? Description { get; set; }   // e.g., "Standard rate", "Holiday promo"
        public bool IsActive { get; set; } = true;
        public DateTime? ValidFrom { get; set; }   // Optional validity period
        public DateTime? ValidTo { get; set; }
    }
    public class Customer : BaseEntity
    {
        // ---------------- Identity ----------------
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;

        // ---------------- Customer Tier / Segment ----------------
        public CustomerTier Tier { get; set; } = CustomerTier.Bronze; // default
        public string? SegmentId { get; set; }   // optional: dynamic segment from DB
        public CustomerSegment? Segment { get; set; }

        // ---------------- Rewards ----------------
        public decimal RewardPoints { get; set; } = 0; // current points balance

        // ---------------- Purchase History ----------------
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        // ---------------- Payment / Ledger ----------------
        public ICollection<PaymentTransaction> Payments { get; set; } = new List<PaymentTransaction>();

        // ---------------- Returns / Refunds ----------------
        public ICollection<SaleReturn> SaleReturns { get; set; } = new List<SaleReturn>();

        // ---------------- Utility / Computed Properties ----------------

        /// <summary>
        /// Total amount purchased (Base currency)
        /// </summary>
        // public decimal TotalPurchaseAmount => Purchases.Sum(p => p.BaseAmount);

        /// <summary>
        /// Total due amount
        /// </summary>
        // public decimal TotalDue => Purchases.Sum(p => p.DueAmount);

        /// <summary>
        /// Current stock of returned items (if you want per customer stock ledger)
        /// </summary>
        // public decimal CurrentStock => ... optional
    }
    public enum CustomerTier
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    public class CustomerSegment : BaseEntity
    {
        public string Name { get; set; } = null!;
        public decimal MinPurchase { get; set; }
        public decimal MaxPurchase { get; set; }
        public decimal RewardMultiplier { get; set; } = 1;
        public bool IsActive { get; set; } = true;
    }

}