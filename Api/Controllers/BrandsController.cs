
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController(
    ILogger<BrandsController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query query)
    {
        logger.LogInformation("Fetching brands result");
        var queryable = context.Brands.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            queryable = queryable.Where(b => b.Name.ToLower().Contains(query.Search.ToLower()));
        }
        var totalCount = await queryable.CountAsync();

        var brands = await queryable
            .OrderBy(b => b.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new GetBrand(x.Id, x.Name, x.Description))
            .ToListAsync();

        var result = new Paging<GetBrand>()
        {
            Data = brands,
            Page = query.Page,
            PageSize = query.PageSize,
            Total = totalCount
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public  async Task<ActionResult> GetById(string id)
    {
        var find = await context.Brands.FindAsync(id);
        if (find is null)
            return NotFound();
        return Ok(new GetBrand(find.Id, find.Name, find.Description));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateBrandRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var brand = new Entities.Brand { Id = Guid.NewGuid().ToString(), Name = request.Name, Description = request.Description };
        await context.Brands.AddAsync(brand);
        await context.SaveChangesAsync();
        return Ok(new GetBrand(brand.Id, brand.Name, brand.Description));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateBrandRequest request)
    {

        var find = await context.Brands.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }

        find.Name = request.Name;
        find.Description = request.Description;
        await context.SaveChangesAsync();
        return Ok(find);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.Brands.FindAsync(id);
        if (find is null)
            return NotFound();
        context.Brands.Remove(find);
        await context.SaveChangesAsync();
        return Ok();
    }
}