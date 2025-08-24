using Api.Entities;
using Api.Mapping;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    ILogger<ProductsController> logger,
    StoreDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductQuery request)
    {
        logger.LogInformation(
            "Querying products with search: {Search}, page: {Page}, pageSize: {PageSize}",
            request.Search, request.Page, request.PageSize
        );
        var query = dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(b => b.Name.ToLower().Contains(request.Search.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var result = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Attributes)
            .Select(x => x.AsDto())
            .ToListAsync();

        var pageResult = new Paging<GetProduct>()
        {
            Data = result,
            Page = request.Page,
            PageSize = request.PageSize,
            Total = totalCount
        };
        logger.LogInformation(
            "Querying products result: {Search}, page: {Page}, pageSize: {PageSize} total: {Total}",
            pageResult.Data, pageResult.Page, pageResult.PageSize, totalCount);
        return Ok(pageResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        logger.LogInformation("Request for product by id: {id}", id);

        var result = await dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Attributes)
            .Select(x => x.AsDto())
            .FirstOrDefaultAsync(p => p.Id == id);

        if (result == null)
        {
            logger.LogWarning("Product {ProductId} not found", id);
            return NotFound();
        }

        logger.LogInformation("product found result by id: {id}", id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateProduct request)
    {
        logger.LogInformation("Creating product {Product} with {attributes} attributes", request.Name,
            request.Attributes?.Count ?? 0);

        var brand = await dbContext.Brands.AsNoTracking().FirstOrDefaultAsync(f => f.Id == request.BrandId);
        if (brand is null)
        {
            return BadRequest($"Brand with ID {request.BrandId} was not found.");
        }

        var category = await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(f => f.Id == request.CategoryId);
        if (category is null)
        {
            return BadRequest($"Category with ID {request.CategoryId} was not found.");
        }

        var product = new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            LocalName = request.LocalName,
            PartNo = request.PartNo,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            Notes = request.Notes,
            Attributes = []
        };
        product.Id = Guid.NewGuid().ToString();

        if (request.Attributes != null)
            foreach (var attribute in request.Attributes)
            {
                product.Attributes.Add(new()
                {
                    Id = Guid.NewGuid().ToString(),
                    AttributeName = attribute.AttributeName,
                    AttributeValue = attribute.AttributeValue
                });
                logger.LogTrace("Added new attribute {AttributeId} to variation {VariationId}",
                    attribute.Id, product.Id);
            }

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var response = new GetProduct(
          product.Id,
          product.Name,
          product.LocalName,
          product.PartNo,
          new EntityRef(product.BrandId, brand.Name),
          new EntityRef(product.CategoryId, category.Name),
          product.Attributes.Select(a => new GetProductAttribute(a.Id, a.AttributeName, a.AttributeValue)).ToList(),
          product.Notes
      );

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] CreateProduct request)
    {
        logger.LogInformation("Updating product {ProductId} with {attributes} attributes", id, request.Attributes?.Count ?? 0);

        var existingProduct = await dbContext.Products
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingProduct == null)
        {
            logger.LogWarning("Product {ProductId} not found for update", id);
            return NotFound();
        }

        var brand = await dbContext.Brands.AsNoTracking().FirstOrDefaultAsync(f => f.Id == request.BrandId);
        if (brand is null)
        {
            return BadRequest($"Brand with ID {request.BrandId} was not found.");
        }

        var category = await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(f => f.Id == request.CategoryId);
        if (category is null)
        {
            return BadRequest($"Category with ID {request.CategoryId} was not found.");
        }


        existingProduct.Name = request.Name;
        existingProduct.LocalName = request.LocalName;
        existingProduct.PartNo = request.PartNo;
        existingProduct.BrandId = request.BrandId;
        existingProduct.CategoryId = request.CategoryId;
        existingProduct.Notes = request.Notes;

        var existingAttributes = existingProduct.Attributes.ToDictionary(x => x.Id);
        if (request.Attributes != null)
            foreach (var attribute in request.Attributes)
            {
                if (!string.IsNullOrEmpty(attribute.Id) &&
                    existingAttributes.TryGetValue(attribute.Id, out var existingAttribute))
                {
                    existingAttribute.AttributeName = attribute.AttributeName;
                    existingAttribute.AttributeValue = attribute.AttributeValue;
                    logger.LogTrace("Updated attribute {AttributeId} in product {productId}",
                        existingAttribute.Id, id);
                }
                else
                {
                    var variationAttribute = new ProductAttribute
                    {
                        Id = Guid.NewGuid().ToString(),
                        AttributeName = attribute.AttributeName,
                        AttributeValue = attribute.AttributeValue
                    };

                    logger.LogTrace("Added new attribute {product} to product {VariationId}",
                        id, variationAttribute.Id);
                }
            }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Successfully updated product {ProductId}", id);

        var response = new GetProduct(
        existingProduct.Id,
        existingProduct.Name,
        existingProduct.LocalName,
        existingProduct.PartNo,
        new EntityRef(existingProduct.BrandId, existingProduct.Name),
        new EntityRef(existingProduct.CategoryId, existingProduct.Name),
        existingProduct.Attributes.Select(a => new GetProductAttribute(a.Id, a.AttributeName, a.AttributeValue)).ToList(),
        existingProduct.Notes
    );
        return Ok(response);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Deleting request product by {id}", id);
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            logger.LogWarning("Deleting request product {id} not found", id);
            return NotFound();
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Deleting request product by {id} successful", id);
        return NoContent();
    }
}