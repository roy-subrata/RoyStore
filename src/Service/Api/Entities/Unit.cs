namespace Api.Entities;

public class Unit:BaseEntity
{
    public string Name { get; set; } = null!;  // "Piece", "Box", "Kg", etc.

    public string? Symbol { get; set; }        // "pc", "box", "kg"

    public ICollection<ItemUnit> ItemUnits { get; set; } = [];
}