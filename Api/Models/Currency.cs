namespace Api.Models;

public record CreateCurrencyRequest(string CurrencyCode, string CurrencyName, decimal ExchangeRateToBase, bool IsBaseCurrency);
public record UpdateCurrencyRequest(string Id, string CurrencyCode, string CurrencyName, decimal ExchangeRateToBase, bool IsBaseCurrency);
public record GetCurrencyResponse(string Id, string CurrencyName, string CurrencyCode, decimal ExchangeRateToBase, bool IsBaseCurrency);