namespace Api.Models;

public class BrandQuery : Query { }
public record GetBrand(string Id,string Name);
public record CreateBrand(string Name);
public record UpdateBrand(string Id,string Name);




