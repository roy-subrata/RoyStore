
namespace Api.Models;

public class ProductQuery : Query { }
public record EntityRef(string Id, string Name){};
public record GetProduct(
    string Id, 
    string Name,
    string LocalName,
    string PartNo,
    EntityRef Brand,
    EntityRef Category,
    List<GetProductAttribute> Attributes,
    string Notes);

public record GetProductAttribute(
    string Id,
    string AttributeName,
    string AttributeValue);

public record CreateProduct(
    string Name,
    string LocalName,
    string PartNo,
    string BrandId,
    string CategoryId,
    string Notes,
    List<CreateProductAttribute> Attributes);



public record CreateProductAttribute(
    string Id, 
    string AttributeName, 
    string AttributeValue);


