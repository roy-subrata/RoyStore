namespace Api.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public string CategoryId { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public string BrandId { get; set; } = null!;
    public Brand Brand { get; set; } = null!;
    public string LocalName { get; set; } = null!;
    public string PartNo { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsActive { get; set; } = true;
    public double StockQuantity { get; set; } = 0;  // Current stock
    public Unit BaseUnit { get; set; } = null!;
    public string UnitId { get; set; } = null!;
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<ProductFeature> ProductFeatures { get; set; } = new List<ProductFeature>();
}