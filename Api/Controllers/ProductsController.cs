using Api.Entities;
using Api.Entities.Experiment;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ILogger<ProductsController> logger, StoreDbContext dbContext)
    : ControllerBase
{
    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Query productQuery)
    {
        logger.LogInformation(
            "Querying products with search: {Search}, page: {Page}, pageSize: {PageSize}",
            productQuery.Search, productQuery.Page, productQuery.PageSize
        );
        var query = dbContext.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(productQuery.Search))
        {
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{productQuery.Search}%"));
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((productQuery.Page - 1) * productQuery.PageSize)
            .Take(productQuery.PageSize)
            .Select(p => new GetProductResponse(
                p.Id,
                p.Name,
                p.LocalName,
                p.PartNo,
                p.PurchaseItems
                    .Where(pi => pi.Purchase.Status == PurchaseStatus.ReadyToStock)
                    .Sum(pi => (int?)pi.Quantity) ?? 0,
                    p.Description,
                new EntityRef(p.BrandId, p.Brand.Name),
                new EntityRef(p.CategoryId, p.Category.Name),
                p.ProductFeatures.Select(f => new GetProductFeature(
                    f.FeatureValue.FeatureId,
                    f.FeatureValue.Feature.Name,
                    f.FeatureValue.Value
                )).ToList()
            ))
            .AsNoTracking()
            .ToListAsync();

        return Ok(new Paging<GetProductResponse>
        {
            Data = products,
            Page = productQuery.Page,
            PageSize = productQuery.PageSize,
            Total = totalCount
        });
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var product = await dbContext.Products
            .Where(p => p.Id == id)
            .OrderBy(p => p.Name) // order while still working with entity
            .Select(p => new GetProductResponse(
                p.Id,
                p.Name,
                p.LocalName,
                p.PartNo,
                p.PurchaseItems
                    .Where(pi => pi.Purchase.Status == PurchaseStatus.ReadyToStock)
                    .Sum(pi => (int?)pi.Quantity) ?? 0,
                    p.Description,
                new EntityRef(p.BrandId, p.Brand.Name),
                new EntityRef(p.CategoryId, p.Category.Name),
                p.ProductFeatures.Select(f => new GetProductFeature(
                    f.FeatureValue.FeatureId,
                    f.FeatureValue.Feature.Name,
                    f.FeatureValue.Value
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        if (product == null) return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateProductRequest request)
    {
        var brand = await dbContext.Brands.FindAsync(request.BrandId);
        if (brand == null)
            return BadRequest(new { error = $"Brand with ID {request.BrandId} was not found." });

        var category = await dbContext.Categories.FindAsync(request.CategoryId);
        if (category == null)
            return BadRequest(new { error = $"Category with ID {request.CategoryId} was not found." });

        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            LocalName = request.LocalName,
            PartNo = request.PartNo,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Description = request.Description,
            ProductFeatures = new List<ProductFeature>()
        };

        if (request.Features.Any())
        {
            foreach (var feature in request.Features)
            {
                var findFeature = await dbContext.Features.FindAsync(feature.Id);
                if (findFeature == null) continue;

                var featureValue = await dbContext.FeatureValues
                    .FirstOrDefaultAsync(fv => fv.FeatureId == feature.Id && fv.Value == feature.Value);

                if (featureValue == null)
                {
                    featureValue = new FeatureValue
                    {
                        Id = Guid.NewGuid().ToString(),
                        FeatureId = feature.Id,
                        Value = feature.Value
                    };
                    dbContext.FeatureValues.Add(featureValue);
                }

                // ✅ always link, even if featureValue already exists
                product.ProductFeatures.Add(new ProductFeature
                {
                    ProductId = product.Id,
                    FeatureValueId = featureValue.Id
                });
            }
        }

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var response = new GetProductResponse(
            product.Id,
            product.Name,
            product.LocalName,
            product.PartNo,
            0,
            product.Description,
            new EntityRef(product.BrandId, brand.Name),
            new EntityRef(product.CategoryId, category.Name),
            product.ProductFeatures.Select(f => new GetProductFeature(
                f.FeatureValue.FeatureId,
                f.FeatureValue.Feature.Name,
                f.FeatureValue.Value
            )).ToList()
        );

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                logger.LogWarning("Model validation failed: {Errors}", string.Join(", ", errors));
                return BadRequest(new { error = "Validation failed", details = errors });
            }
            var product = await dbContext.Products
            .Include(p => p.ProductFeatures)
            .ThenInclude(pf => pf.FeatureValue)
            .ThenInclude(featureValue => featureValue.Feature)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { error = $"Product with id {id} not found." });

            var brand = await dbContext.Brands.FindAsync(request.BrandId);
            if (brand == null)
                return BadRequest(new { error = $"Brand with ID {request.BrandId} not found." });

            var category = await dbContext.Categories.FindAsync(request.CategoryId);
            if (category == null)
                return BadRequest(new { error = $"Category with ID {request.CategoryId} not found." });

            // Update basic fields
            product.Name = request.Name;
            product.LocalName = request.LocalName;
            product.PartNo = request.PartNo;
            product.BrandId = request.BrandId;
            product.CategoryId = request.CategoryId;
            product.Description = request.Description;

            // Handle features
            var newFeatureIds = new List<string>();

            if (request.Features.Any())
            {
                foreach (var feature in request.Features)
                {
                    var featureValue = await dbContext.FeatureValues
                        .FirstOrDefaultAsync(fv => fv.FeatureId == feature.Id && fv.Value == feature.Value);

                    if (featureValue == null)
                    {
                        featureValue = new FeatureValue
                        {
                            Id = Guid.NewGuid().ToString(),
                            FeatureId = feature.Id,
                            Value = feature.Value
                        };
                        dbContext.FeatureValues.Add(featureValue);
                    }

                    newFeatureIds.Add(featureValue.Id);

                    if (product.ProductFeatures.All(pf => pf.FeatureValueId != featureValue.Id))
                    {
                        product.ProductFeatures.Add(new ProductFeature
                        {
                            ProductId = product.Id,
                            FeatureValueId = featureValue.Id
                        });
                    }
                }
            }

            // ✅ Remove old features not in the new list
            var featuresToRemove = product.ProductFeatures
                .Where(pf => !newFeatureIds.Contains(pf.FeatureValueId))
                .ToList();

            foreach (var feature in featuresToRemove)
            {
                product.ProductFeatures.Remove(feature);
            }

            await dbContext.SaveChangesAsync();

            var response = new GetProductResponse(
                product.Id,
                product.Name,
                product.LocalName,
                product.PartNo,
                product.PurchaseItems.Sum(x => x.Quantity),
                product.Description,
                new EntityRef(product.BrandId, brand.Name),
                new EntityRef(product.CategoryId, category.Name),
                product.ProductFeatures.Select(pf => new GetProductFeature(
                    pf.FeatureValue.FeatureId,
                    pf.FeatureValue.Feature.Name,
                    pf.FeatureValue.Value
                )).ToList()
            );

            return Ok(response);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product with ID {id}", id);
            return StatusCode(500, new { error = "An error occurred while updating the product." });
        }

    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { error = $"Product with id {id} not found." });

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}