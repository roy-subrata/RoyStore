using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(
    ILogger<CategoryController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CategoryQuery request)
    {
        var query = context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c => c.Name.ToLower().Contains(request.Search.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var categories = await query
            .OrderBy(b => b.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetCategory(x.Id, x.Name))
            .ToListAsync();
        
        var pageResult = new Paging<GetCategory>()
        {
            Data = categories,
            Page = request.Page,
            PageSize = request.PageSize,
            Total = totalCount
        };
        return Ok(pageResult);
    }

    [HttpGet]
    async Task<ActionResult> Get(string id)
    {
        var find = await context.Categories.FindAsync(id);
        if (find is null)
            return NotFound();
        return Ok(new GetCategory(find.Id, find.Name));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCategory request)
    {
        var category = new Category() { Id = Guid.NewGuid().ToString(), Name = request.Name };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateCategory request)
    {
        var find = await context.Categories.FindAsync(id);
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
        var find = await context.Categories.FindAsync(id);
        if (find is null)
            return NotFound();
        context.Categories.Remove(find);
        await context.SaveChangesAsync();
        return Ok();
    }
}