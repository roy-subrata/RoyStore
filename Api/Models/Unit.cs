namespace Api.Models;

public record CreateUnitRequest(string Name, string ShortCode, bool IsBaseUnit);
public record UpdateUnitRequest(string Id, string Name, string ShortCode, bool IsBaseUnit);
public record GetUnitResponse(string Id, string Name, string ShortCode, bool IsBaseUnit);