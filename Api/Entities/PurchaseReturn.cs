namespace Api.Entities;

public class PurchaseReturn : BaseEntity
{
    public string? PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;
    public DateTime ReturnDate { get; set; }
    public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
}