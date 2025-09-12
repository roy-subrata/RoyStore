namespace Api.Models;

public record CreateUnitRequest(string Name, string ShortCode, decimal ConversionToBase);
public record UpdateUnitRequest(string Id, string Name, string ShortCode, decimal ConversionToBase);
public record GetUnitResponse(string Id, string Name, string ShortCode, decimal ConversionToBase);