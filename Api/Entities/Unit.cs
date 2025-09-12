using Api.Entities.Experiment;

namespace Api.Entities;


public class Unit : BaseEntity
{
    public string Name { get; set; } = null!; // e.g., "Piece", "Box", "Kilogram"
    public string ShortCode { get; set; } = null!; // e.g., "pc", "box", "kg"
    public decimal ConversionToBase { get; set; } = 1; // How many base units in this unit
}