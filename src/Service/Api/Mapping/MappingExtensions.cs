using Api.Entities;
using Api.Models;

namespace Api.Mapping;

public static class MappingExtensions
{
    public static GetProduct AsDto(this Product product)
    {
        return new GetProduct(
            product.Id,
            product.Name,
            product.LocalName,
            product.PartNo ?? "",
            product.Brand.AsDto(),
            product.Category.AsDto(),
            product.Attributes.Select(x => x.AsDto()).ToList(),
            product.Notes
        );
    }
    public static GetStock AsDto(this PurchaseItem item)
    {
        return new GetStock(
            item.Id,
            item.Purchase.PurchaseNumber,
            new EntityRef(item.Purchase.Supplier.Id, item.Purchase.Supplier.Name),
            new GetProduct(
                item.ProductId,
                item.Product.Name,
                item.Product.LocalName,
                item.Product.PartNo ?? string.Empty,
                new EntityRef(item.Product.Category.Id, item.Product.Category.Name),
                new EntityRef(item.Product.Brand.Id, item.Product.Brand.Name),
                item.Product.Attributes
                    .Select(x => new GetProductAttribute(x.Id, x.AttributeName, x.AttributeValue))
                    .ToList(),
                item.Product.Notes
            ),
            item.UnitPrice,
            item.RemainingQuantity
        );
    }

    public static EntityRef AsDto(this Brand brand)
    {
        return new EntityRef(brand.Id, brand.Name);
    }

    public static EntityRef AsDto(this Category category)
    {
        return new EntityRef(category.Id, category.Name);
    }

    public static EntityRef AsDto(this Supplier supplier)
    {
        return new EntityRef(supplier.Id, supplier.Name);
    }

    public static GetProductAttribute AsDto(this ProductAttribute attribute)
    {
        return new GetProductAttribute(
            attribute.Id,
            attribute.AttributeName,
            attribute.AttributeValue);
    }


}

