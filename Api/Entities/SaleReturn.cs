// using Api.Entities.Experiment;

// namespace Api.Entities;

// public class SaleReturn:BaseEntity
// {
//     public int SaleId { get; set; }
//     public Sale Sale { get; set; } = null!;

//     public DateTime ReturnDate { get; set; }
//     public string? Reason { get; set; }

//     public ICollection<PaymentTransaction> Payments { get; set; } = [];


// }

namespace Api.Entities;

public class SaleReturn : BaseEntity
{
    public string? SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public DateTime ReturnDate { get; set; }
    public ICollection<SaleReturnItem> Items { get; set; } = new List<SaleReturnItem>();
}