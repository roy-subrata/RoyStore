
namespace Api.Models;

public record EntityRef(string Id, string Name) { };
public record GetProductResponse(
    string Id,
    string Name,
    string LocalName,
    string PartNo,
    decimal StockQuantity,
    string Description,
    EntityRef Brand,
    EntityRef Category,
    EntityRef Unit,
    List<GetProductFeature> Features);

public record GetProductFeature(
    string Id,
    string Name,
    string Value);

public record CreateProductRequest(
    string Name,
    string LocalName,
    string PartNo,
    string BrandId,
    string CategoryId,
      string UnitId,
    string Description,
    List<CreateProductFeature> Features);

public record UpdateProductRequest(
    string Id,
    string Name,
    string LocalName,
    string PartNo,
    string BrandId,
    string CategoryId,
    string UnitId,
    string Description,
    List<CreateProductFeature> Features);

public record CreateProductFeature(
    string Id,
    string Value);


