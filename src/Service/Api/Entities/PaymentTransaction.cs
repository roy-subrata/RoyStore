namespace Api.Entities;

public sealed class PaymentTransaction:BaseEntity
{
    
    public string TransactionType { get; set; } = null!; // "IN" or "OUT"
    
    public string PartyType { get; set; } = null!; // "Customer" or "Supplier"

    public int PartyId { get; set; } // ID from Customer or Supplier

    public int? SaleId { get; set; }
    public Sale? Sale { get; set; }
    
        
    public int? SaleReturnId { get; set; }
    public SaleReturnItem? SaleReturn { get; set; }

    public int? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    
    public int? PurchaseReturnId { get; set; }
    public PurchaseReturn? PurchaseReturn { get; set; }


    public DateTime PaymentDate { get; set; }

    public decimal AmountPaid { get; set; }
    
    public string CurrencyCode { get; set; } = "BDT";

    public decimal ExchangeRate { get; set; } = 1;

    public decimal BaseAmount => Math.Round(AmountPaid * ExchangeRate, 2);

    public string? PaymentMethod { get; set; } // e.g. "Cash", "bKash"

    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Currency? Currency { get; set; }
}