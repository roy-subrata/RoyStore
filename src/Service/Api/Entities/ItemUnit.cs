namespace Api.Entities;

public class ItemUnit:BaseEntity
{
    public string ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public decimal ConversionRateToBase { get; set; } // 1 box = 12 pcs → 12
    public bool IsBaseUnit { get; set; } = false;
}