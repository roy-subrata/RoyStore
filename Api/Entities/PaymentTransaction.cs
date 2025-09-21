namespace Api.Entities;

public class PaymentTransaction : BaseEntity
{
    public PartyType PartyType { get; set; }             // Customer / Supplier
    public string PartyId { get; set; } = null!;         // FK to Customer/Supplier

    public string? SaleId { get; set; }
    public Sale? Sale { get; set; }

    public string? SaleReturnId { get; set; }
    public SaleReturn? SaleReturn { get; set; }

    public string PurchaseId { get; set; } = null!;
    public Purchase Purchase { get; set; } = null!;

    public string? PurchaseReturnId { get; set; }
    public PurchaseReturn? PurchaseReturn { get; set; }

    public string PaymentMethodId { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;

    // ---------------- Currency Fields ----------------
    public string CurrencyCode { get; set; } = "BDT";   // Payment currency
    public double ExchangeRate { get; set; } = 1;      // Conversion to base currency
    public double AmountPaid { get; set; }             // Amount in CurrencyCode
    public double BaseAmount => Math.Round(AmountPaid * ExchangeRate, 2); // Base currency

    public int? CurrencyId { get; set; }
    public Currency? Currency { get; set; }            // Navigation property

    // ---------------- Ledger & Advance Fields ----------------
    public bool IsAdvance { get; set; } = false;       // Payment in advance
    public bool IsAdjustment { get; set; } = false;    // Used to apply advance/refund

    public DateTime PaymentDate { get; set; }
    public string? NoteRef { get; set; }
    public string? Notes { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}