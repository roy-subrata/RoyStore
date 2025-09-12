namespace Api.Entities;

public class Purchase : BaseEntity
{
    public string PurchaseNumber { get; set; } = null!;
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }

    public PurchaseStatus Status { get; set; }

    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}