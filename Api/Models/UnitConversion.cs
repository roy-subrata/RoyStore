namespace Api.Models;

public record CreateUnitConversionRequest(string FromUnitId, string ToUnitId, double Factor);
public record UpdateUnitConversionRequest(string Id,string FromUnitId, string ToUnitId, double Factor);
public record GetUnitConversionResponse(string Id, EntityRef FromUnit, EntityRef ToUnit, double Factor);