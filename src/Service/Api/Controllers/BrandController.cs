
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController(
    ILogger<BrandController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] BrandQuery request)
    {
        var query =  context.Brands.AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(b => b.Name.ToLower().Contains(request.Search.ToLower()));
        }
        var totalCount = await query.CountAsync();
        
        var brands = await query
            .OrderBy(b => b.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetBrand(x.Id, x.Name))
            .ToListAsync();
        
        var pageResult = new Paging<GetBrand>()
        {
            Data = brands,
            Page = request.Page,
            PageSize = request.PageSize,
            Total = totalCount
        };
        return Ok(pageResult);
    }

    [HttpGet("{id}")]
    async Task<ActionResult> Get(string id)
    {
        var find = await context.Brands.FindAsync(id);
        if (find is null)
            return NotFound();
        return Ok(new GetBrand(find.Id, find.Name));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateBrand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var brand = new Entities.Brand { Id = Guid.NewGuid().ToString(), Name = request.Name };
        await context.Brands.AddAsync(brand);
        await context.SaveChangesAsync();
        return Ok(new GetBrand(brand.Id, brand.Name));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateBrand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var find = await context.Brands.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }

        find.Name = request.Name;
        await context.SaveChangesAsync();
        return Ok();
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