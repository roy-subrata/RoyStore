using Api.Entities.Experiment;

namespace Api.Entities;

public class Currency : BaseEntity
{
    public string CurrencyCode { get; set; } = null!; // e.g., "USD", "BDT"
    public string CurrencyName { get; set; } = null!;
    public decimal ExchangeRateToBase { get; set; }  // 1.00 for base
    public bool IsBaseCurrency { get; set; } = false;
}