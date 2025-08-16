namespace Api.Models;

public class UnitQuery : Query { }
public record CreateUnit(string Name,string Symbol);
public record UpdateUnit(string Id,string Name,string Symbol);
public record GetUnit(string Id,string Name,string Symbol);