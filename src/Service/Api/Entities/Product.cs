namespace Api.Entities;

using System;
using System.Collections.Generic;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string LocalName { get; set; }
    public string? PartNo { get; set; } 
    public string BrandId { get; set; }
    public Brand Brand { get; set; }
    public string CategoryId { get; set; }
    public Category Category { get; set; }
    public string Notes { get; set; }
    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
}

public class ProductAttribute : BaseEntity
{
    public string ProductId { get; set; }
    
    public Product Product { get; set; }
    public string AttributeName { get; set; }
    public string AttributeValue { get; set; }
}
public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}