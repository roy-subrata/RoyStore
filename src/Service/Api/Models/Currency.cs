namespace Api.Models;

public class CurrencyQuery : Query { }
public record CreateCurrency(string CurrencyCode, string CurrencyName, decimal ExchangeRateToBase ,bool IsBaseCurrency);
public record UpdateCurrency(string Id,string CurrencyCode, string CurrencyName, decimal ExchangeRateToBase ,bool IsBaseCurrency);
public record GetCurrency(string Id,string CurrencyName,string CurrencyCode,decimal ExchangeRateToBase ,bool IsBaseCurrency);