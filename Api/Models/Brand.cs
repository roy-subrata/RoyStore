namespace Api.Models;
public record GetBrand(string Id,string Name,string Description);
public record CreateBrandRequest(string Name,string Description);
public record UpdateBrandRequest(string Name,string Description);




