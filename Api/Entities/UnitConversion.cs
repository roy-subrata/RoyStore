namespace Api.Entities;

public class UnitConversion : BaseEntity
{
    public string FromUnitId { get; set; }
    public Unit FromUnit { get; set; } = null!;

    public string ToUnitId { get; set; }
    public Unit ToUnit { get; set; } = null!;

    public double Factor { get; set; } // e.g., 1 Box = 10 Piece → Factor = 10
}